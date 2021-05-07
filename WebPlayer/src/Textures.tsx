/* eslint-disable @typescript-eslint/no-unused-vars */
import { useApp } from "@inlet/react-pixi";
import { useContext, useEffect, useState } from "react";
import * as PIXI from "pixi.js";
import React from "react";
import { PartituraContext } from "./Partitura";
import { Spritesheet } from "pixi.js";

export const TextureContext = React.createContext<Spritesheet | null>(null);
TextureContext.displayName = "TextureContext";

export const Textures = ({ children }: any) => {
	const app = useApp();
	const partitura = useContext(PartituraContext);
	const [isTextureLoaded, setIsTextureLoaded] = useState(false);
	const [spritesheet, setSpritesheet] = useState<PIXI.Spritesheet | null>(null);

	useEffect(() => {
		//TODO for all pages
		if (!partitura) return;

		const sheetData: PIXI.ISpritesheetData = { frames: {}, meta: { scale: "1" } };
		if (partitura && partitura.Pages.length) {
			partitura.Pages[0].Boxes.forEach(
				(b, i) =>
					(sheetData.frames["b" + i] = {
						frame: b.TextureCoords
					})
			);
		}

		if (app.loader.resources.test && app.loader.resources.test.texture) {
			const sheet = new PIXI.Spritesheet(app.loader.resources.test.texture, sheetData);
			sheet.parse(() => {
				setSpritesheet(sheet);
			});
		} else
			app.loader.add("test", "/La Catedral-000.jpg", (loader: any) => {
				const sheet = new PIXI.Spritesheet(loader.texture, sheetData);
				sheet.parse(() => {
					setSpritesheet(sheet);
				});
			});
		app.loader.load(() => setIsTextureLoaded(true));
	}, [app.loader, partitura]);

	if (!isTextureLoaded) {
		return <div>Loading...</div>;
	} else if (!spritesheet || !spritesheet.textures.hasOwnProperty("b0")) {
		return <div>Couldn't load spritesheet</div>;
	} else {
		return <TextureContext.Provider value={spritesheet}>{children}</TextureContext.Provider>;
	}
};
