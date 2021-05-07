/* eslint-disable @typescript-eslint/no-unused-vars */
import { useApp } from "@inlet/react-pixi";
import { useEffect, useState } from "react";
import React from "react";

interface ActivePartitura {
	Name: string;
	Pages: Page[];
}
interface Page {
	Boxes: Box[];
	ImagePath: string;
}
interface Box {
	TextureCoords: TextureCoords;
	Coords: Coords;
}
interface Coords {
	X: number;
	Y: number;
	Width: number;
	Height: number;
	CenterY: number;
}
interface TextureCoords {
	x: number;
	y: number;
	w: number;
	h: number;
	centerY: number;
}

const toImageCoordinates = (coords: Coords): TextureCoords => {
	//todo get from image properties
	const screenCordinatesFactor = 600 / 96;

	return {
		x: coords.X * screenCordinatesFactor,
		y: coords.Y * screenCordinatesFactor,
		w: coords.Width * screenCordinatesFactor,
		h: coords.Height * screenCordinatesFactor,
		centerY: coords.CenterY * screenCordinatesFactor
	};
};

export const PartituraContext = React.createContext<ActivePartitura | null>(null);
PartituraContext.displayName = "PartituraContext";

export const Partitura = ({ children }: any) => {
	const app = useApp();
	const [error, setError] = useState();
	const [isPartituraLoaded, setIsPartituraLoaded] = useState(false);
	const [partitura, setPartitura] = useState<ActivePartitura | null>(null);

	useEffect(() => {
		fetch("/La Catedral.partitura2")
			.then(res => res.json())
			.then(
				(result: ActivePartitura) => {
					setIsPartituraLoaded(true);
					result.Name = "La Catedral";
					result.Pages.forEach((p: Page, i: number) => {
						p.ImagePath = "/" + result.Name + "-00" + i + ".jpg";
						p.Boxes.map(b => (b.TextureCoords = toImageCoordinates(b.Coords)));
					});
					setPartitura(result);
				},
				error => {
					setIsPartituraLoaded(true);
					setError(error);
				}
			);
	}, []);

	if (error) {
		return <div>Failed to load partitura: {error}</div>;
	} else if (!isPartituraLoaded) {
		return <div>Loading...</div>;
	} else if (!partitura) {
		return <div>Couldn't load partitura</div>;
	} else {
		return <PartituraContext.Provider value={partitura}>{children}</PartituraContext.Provider>;
	}
};
