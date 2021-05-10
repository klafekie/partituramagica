/* eslint-disable @typescript-eslint/no-unused-vars */
import { Stage, Container, Text, Sprite, useApp } from "@inlet/react-pixi";
import { useEffect, useContext, useRef } from "react";
import * as PIXI from "pixi.js";
import { TextureContext } from "./Textures";
import { PlayerContext } from "./Player";
import { ConfigurationContext } from "./Configuration";
import { PlayablePartituraContext } from "./PlayablePartitura";

export const PixiStage = () => {
	const { fontSize, textColor } = useContext(ConfigurationContext).read().stage;
	const pixiApp = useApp();
	const { screen, renderer } = pixiApp;
	const textures = useContext(TextureContext);
	const playablePartitura = useContext(PlayablePartituraContext);
	const player = useContext(PlayerContext);

	const stageWrapperRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		console.log(
			"resize to ",
			document.documentElement.clientWidth,
			document.documentElement.clientHeight - 21
		);

		renderer.resize(
			document.documentElement.clientWidth,
			document.documentElement.clientHeight - 21
		);
		window.addEventListener("resize", function () {
			console.log(
				"resize to ",
				document.documentElement.clientWidth,
				document.documentElement.clientHeight - 21
			);
			renderer.resize(
				document.documentElement.clientWidth,
				document.documentElement.clientHeight - 21
			);
		});
	}, [renderer]);

	if (!player || !textures || !playablePartitura)
		return (
			<Text
				text="Loading ..."
				style={new PIXI.TextStyle({ fontSize: fontSize, fill: textColor })}></Text>
		);

	const { ticker, offsetY, zoom, meta } = player;
	return (
		<div ref={stageWrapperRef}>
			<Stage
				width={screen.width}
				height={screen.height}
				options={{
					backgroundColor: 0xdddddd
				}}>
				<Container>
					{playablePartitura.boxes.map((box, i) => {
						const texture =
							textures.pages[box.pageIndex].boxes[box.lineIndex].splitBoxes[box.splitterIndex]
								.texture;

						const sprite = (
							<Sprite
								key={"" + i}
								texture={texture}
								x={(box.x + ticker.currentXPos) * zoom.x + screen.width / 2}
								y={screen.height / 2 + box.y * zoom.y + offsetY}
								width={box.w * zoom.x}
								height={box.h * zoom.y}></Sprite>
						);

						return sprite;
					})}
				</Container>
				<Text
					text={meta.positionDescription}
					style={new PIXI.TextStyle({ fontSize: fontSize, fill: textColor })}></Text>
			</Stage>
		</div>
	);
};
