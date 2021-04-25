/* eslint-disable @typescript-eslint/no-unused-vars */
import { useTick, Container, Text, Sprite, useApp } from "@inlet/react-pixi";
import { ActivePartitura, Coords } from "./Partitura";
import { useEffect, useState } from "react";
import * as PIXI from "pixi.js";

interface Props {
	activePartitura: ActivePartitura;
}

export const Player = ({ activePartitura }: Props) => {
	const [xPos, setXPos] = useState(0);
	const [spritesheet, setSpritesheet] = useState<PIXI.Spritesheet | null>(null);
	const app = useApp();

	const sheetData: PIXI.ISpritesheetData = { frames: {}, meta: { scale: '1' } };
	if (activePartitura && activePartitura.Pages.length) {
		activePartitura.Pages[0].Boxes.forEach(
			(b, i) =>
				(sheetData.frames["b" + i] = {
					frame: b.TextureCoords,
				})
		);
	}

	useEffect(() => {
		//TODO for all pages
		if (app.loader.resources.test) return;

		app.loader.add("test", "/La Catedral-000.jpg", (loader: any) => {
			const sheet = new PIXI.Spritesheet(loader.texture, sheetData);
			sheet.parse(() => {
				setSpritesheet(sheet);
			});
		});
		app.loader.load();
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [activePartitura]);

	useEffect(() => {
		app.renderer.resize(window.innerWidth, window.innerHeight);
		window.addEventListener("resize", function () {
			app.renderer.resize(window.innerWidth, window.innerHeight);
		});
	}, [spritesheet, app.renderer]);

	useTick((delta) => setXPos((r) => r - 6 + +0.01 * (delta || 0)));

	let rightMost = xPos;

	return !spritesheet ? (
		<Text
			text="Loading ..."
			style={new PIXI.TextStyle({ fontSize: 50 })}
		></Text>
	) : (
		<Container>
			{activePartitura &&
				spritesheet &&
				activePartitura.Pages[0].Boxes.map((box, i) => {
					const scale = 0.7;	
					const texture = spritesheet.textures["b" + i];					
					const y = app.renderer.screen.height / 2 - box.TextureCoords.centerY * scale;
					const sprite = (
						<Sprite
							key={"b" + i}
							texture={texture}
							x={rightMost}
							y={y}
							scale={scale}
						></Sprite>
					);
					rightMost += spritesheet.textures["b" + i].width * scale;
					return sprite;
				})}
		</Container>
	);
};
