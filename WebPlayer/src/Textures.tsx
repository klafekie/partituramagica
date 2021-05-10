/* eslint-disable @typescript-eslint/no-unused-vars */
import { useApp } from "@inlet/react-pixi";
import { useContext, useEffect, useState } from "react";
import * as PIXI from "pixi.js";
import React from "react";
import { PartituraContext } from "./Partitura";
import { Texture } from "pixi.js";

declare type Dict<T> = {
	[key: string]: T;
};

interface ContextData {
	pages: PageData[];
}
interface PageData {
	boxes: BoxData[];
}
interface SplitBoxData {
	texture: Texture;
}
interface BoxData {
	splitBoxes: SplitBoxData[];
}

export const TextureContext = React.createContext<ContextData | null>(null);
TextureContext.displayName = "TextureContext";

function PageKey(pI: number) {
	return "page-" + pI;
}
function BoxKey(pI: number, bI: number, tI: number) {
	return "box-" + pI + "-" + bI + "-" + tI;
}

export const Textures = ({ children }: any) => {
	const { loader } = useApp();
	const partitura = useContext(PartituraContext);
	const [areTexturesLoaded, setAreTexturesLoaded] = useState(false);
	const [spritesheets, setSpritesheets] = useState<Dict<PIXI.Spritesheet>>({});
	const [areSpritesheetsParsed, setAreSpritesheetsParsed] = useState(false);
	const { ticker } = useApp();

	useEffect(() => {
		if (!partitura || !loader || areTexturesLoaded) return;
		console.log("Texture.useEffect() load base textures", areTexturesLoaded, loader, partitura);

		partitura.pages.forEach((p, pI) => {
			console.log("adding base Texture for page " + pI + " to loader.");
			const indexString = pI.toLocaleString("en-US", { minimumIntegerDigits: 3 });
			loader.add(PageKey(pI), "/2048 La Catedral-" + indexString + ".png", () => {
				console.log("base Texture for page " + pI + " loaded successfully.");
			});
		});
		loader.load(() => setAreTexturesLoaded(true));
	}, [areTexturesLoaded, loader, partitura]);

	useEffect(() => {
		if (!areTexturesLoaded || !partitura || areSpritesheetsParsed) return;
		console.log("Texture.useEffect() create and parse spritesheets");

		let allSheets = partitura.pages.map((p, pI) => {
			const sheetData: PIXI.ISpritesheetData = { frames: {}, meta: { scale: "1" } };
			p.boxes.forEach((b, bI) => {
				b.textureCoords.forEach((t, tI) => {
					sheetData.frames[BoxKey(pI, bI, tI)] = {
						frame: t
					};
				});
			});

			return sheetData;
		});

		const newSpritesheets = { ...spritesheets };
		allSheets.forEach((sheetData, pI) => {
			const resource: PIXI.ILoaderResource = loader.resources[PageKey(pI)];

			if (!resource || !resource.texture) throw new Error("texture not loaded");

			const sheet = new PIXI.Spritesheet(resource.texture, sheetData);
			sheet.parse(() => {
				newSpritesheets[PageKey(pI)] = sheet;
			});
		});
		setSpritesheets(newSpritesheets);
		if (Object.keys(newSpritesheets).length === partitura.pages.length) {
			setAreSpritesheetsParsed(true);
		}
	}, [areSpritesheetsParsed, areTexturesLoaded, loader.resources, partitura, spritesheets]);

	useEffect(() => {
		if (areSpritesheetsParsed && !ticker.started) {
			console.log("starting ticker");
			ticker.start();
		}
	}, [areSpritesheetsParsed, ticker]);

	if (!partitura || !areTexturesLoaded || !areSpritesheetsParsed) {
		return <div>Loading...</div>;
	} else {
		const pages = partitura.pages.map((p, pI) => {
			const boxes = p.boxes.map((b, bI) => {
				const splitBoxes = b.textureCoords.map((t, tI) => {
					return { texture: spritesheets[PageKey(pI)].textures[BoxKey(pI, bI, tI)] };
				});
				return { splitBoxes };
			});
			return { boxes };
		});

		return <TextureContext.Provider value={{ pages: pages }}>{children}</TextureContext.Provider>;
	}
};
