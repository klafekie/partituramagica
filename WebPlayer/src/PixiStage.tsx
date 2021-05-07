/* eslint-disable @typescript-eslint/no-unused-vars */
import { Stage, Container, Text, Sprite, useApp } from "@inlet/react-pixi";
import { useEffect, useContext } from "react";
import * as PIXI from "pixi.js";
import { TextureContext } from "./Textures";
import { PlayerContext } from "./Player";
import { Spritesheet } from "pixi.js";
import { PartituraContext } from "./Partitura";

export const PixiStage = () => {
	const app = useApp();
	const spritesheet = useContext(TextureContext);
	const player = useContext(PlayerContext);
	const partitura = useContext(PartituraContext);

	useEffect(() => {
		app.renderer.resize(window.innerWidth, window.innerHeight);

		window.addEventListener("resize", function () {
			app.renderer.resize(window.innerWidth, window.innerHeight);
		});
	}, [app.renderer]);

	if (!player || !Spritesheet || !partitura)
		return <Text text="Loading ..." style={new PIXI.TextStyle({ fontSize: 50 })}></Text>;

	let rightMost = player.xPos.current;

	return (
		<Stage
			width={window.innerWidth}
			height={window.innerHeight}
			options={{
				backgroundColor: 0xffffff
			}}>
			{spritesheet ? (
				<Container>
					{partitura &&
						partitura.Pages[0].Boxes.map((box, i) => {
							const scale = 0.7;
							const texture = spritesheet.textures["b" + i];
							const y = app.renderer.screen.height / 2 - box.TextureCoords.centerY * scale;
							const sprite = (
								<Sprite key={"b" + i} texture={texture} x={rightMost} y={y} scale={scale}></Sprite>
							);
							rightMost += spritesheet.textures["b" + i].width * scale;
							return sprite;
						})}
				</Container>
			) : (
				<Text text="Loading ..." style={new PIXI.TextStyle({ fontSize: 50 })}></Text>
			)}
		</Stage>
	);
};
