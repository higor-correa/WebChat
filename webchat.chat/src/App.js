import { useEffect, useState } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import axios from "axios";

import "./App.css";

function App() {
  const [token, setToken] = useState("");
  const [feedBackMessage, setFeedBackMessage] = useState("");
  const [currentMessage, setCurrentMessage] = useState("");
  const [user, setUser] = useState("");
  const [messages, setMessages] = useState([]);

  const [connectTries, setConnectTries] = useState(0);
  const [chatState, setChatState] = useState({
    connected: false,
    connection: null,
  });

  const createUser = () => {
    if (user.username == "" || user.password == "") {
      setFeedBackMessage("username and password is mandatory");
      return;
    }

    setToken("");
    axios
      .post(`${process.env.REACT_APP_CHAT_HUB}/api/users`, user)
      .then(() => {
        setFeedBackMessage("");
        autenticateUser();
      })
      .catch(() => setFeedBackMessage("username or password is invalid"));
  };

  const autenticateUser = () => {
    setToken("");
    axios
      .post(`${process.env.REACT_APP_CHAT_HUB}/api/login`, user)
      .then(({ data }) => {
        if (chatState.connected) chatState.connection.stop();

        setToken(data.token);
        console.log(data.token);
        setFeedBackMessage("");
      })
      .catch(() => {
        setFeedBackMessage("username or password is invalid");
      });
  };

  const sendMessage = () => {
    if (chatState.connected) {
      chatState.connection.invoke("SendMessage", currentMessage);
      setCurrentMessage("");
    }
  };

  useEffect(() => {
    if ((token || "") == "") return;

    const connection = new HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_CHAT_HUB}/hubs/chat`, {
        accessTokenFactory: () => token,
      })
      .build();

    connection.on("ReceiveMessage", (user, message, date) => {
      setMessages((m) => {
        let messages = [...m, { user, message, date: new Date(date) }].sort(
          (x, y) => y.date - x.date
        );

        if (messages.length >= 50) {
          messages = messages.slice(0, 50);
        }

        return messages;
      });
    });

    connection.onclose(() => {
      setChatState({
        ...chatState,
        connected: false,
        connection: null,
      });
      setConnectTries(0);
    });

    connection
      .start()
      .then((a) => {
        setChatState({ ...chatState, connected: true, connection });
        console.log(a);
      })
      .catch(() => {
        setChatState({
          ...chatState,
          connected: false,
        });
        setTimeout(() => setConnectTries(connectTries + 1), 2000);
      });

    setChatState({ ...chatState, connection });
  }, [connectTries, token]);

  return (
    <div className="App">
      <label htmlFor="username">Username:</label>
      <input
        type="text"
        name="username"
        onChange={({ target }) => setUser({ ...user, username: target.value })}
      />
      <br />
      <label htmlFor="password">Password:</label>
      <input
        type="password"
        name="password"
        onChange={({ target }) => setUser({ ...user, password: target.value })}
      />
      <br />
      <button onClick={autenticateUser}>Login</button>
      <br />
      <button onClick={createUser}>Create Account</button>

      <br />
      {feedBackMessage}

      <hr />

      <br />
      <label htmlFor="message">Message:</label>
      <input
        type="text"
        name="message"
        value={currentMessage}
        onChange={({ target }) => setCurrentMessage(target.value)}
      />
      <button onClick={sendMessage} disabled={(token || "") == ""}>
        Send
      </button>

      {messages.map((x, i) => (
        <p key={`message-${i}`}>{`${x.date.toLocaleString()} ${x.user}: ${
          x.message
        }`}</p>
      ))}
    </div>
  );
}

export default App;
