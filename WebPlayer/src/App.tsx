import { Stage, Sprite, useTick, Container } from "@inlet/react-pixi";
import React, { useState } from "react";
import "./App.css";

const Player = () => {
	const [xPos, setXPos] = useState(0);
	useTick((delta) => setXPos((r) => r - 1 + +0.01 * (delta || 0)));
	return (
		<Container>
			<Sprite image="./La Catedral-000.jpg" x={xPos} y={200} scale={0.3} />
		</Container>
	);
};
const App = () => {
	//const app = useApp();

	return (
		<Stage width={1000}>
			<Player />
		</Stage>
	);
};

export default App;
