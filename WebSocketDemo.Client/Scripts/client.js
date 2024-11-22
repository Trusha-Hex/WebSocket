const socket = new WebSocket("ws://localhost:5000/ws");

socket.onopen = () => {
    console.log("Connected to server");
    socket.send("Hello from the browser!");
};

socket.onmessage = (event) => {
    console.log("Message from server:", event.data);
};

socket.onclose = () => {
    console.log("Connection closed");
};

socket.onerror = (error) => {
    console.error("WebSocket error:", error);
};
