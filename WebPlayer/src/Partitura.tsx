/* eslint-disable @typescript-eslint/no-unused-vars */
import { useEffect, useState } from "react";
import React from "react";
import camelcaseKeys from "camelcase-keys";

interface ActivePartitura {
	name: string;
	partPattern: string;
	pages: Page[];
}
interface Page {
	boxes: Box[];
	imagePath: string;
}
interface Box {
	splitters: { key: number; value: number }[];
	labelIds: number[];
	textureCoords: TextureCoords[];
	coords: Coords;
	id: number;
}
interface Coords {
	x: number;
	y: number;
	width: number;
	height: number;
	centerY: number;
}
interface TextureCoords {
	currentPartIndex?: number;
	x: number;
	y: number;
	w: number;
	h: number;
	centerY: number;
	id: number;
}

//TODO move to playable partitura? problem: it is also used in Texture.tsx
const toImageCoordinates = ({ id, coords, splitters }: Box): TextureCoords[] => {
	//todo get from image properties
	const screenCordinatesFactor = ((600 / 96 / 4962) * 2895) / 2;

	let entries = [{ key: 0, value: id }, ...splitters, { key: coords.width, value: -1 }];

	const splitBoxes = [];
	for (let sI = 0; sI < entries.length - 1; sI++) {
		const entry = entries[sI];
		const width = entries[sI + 1].key - entry.key;
		splitBoxes.push({
			id: entry.value,
			x: (coords.x + entry.key) * screenCordinatesFactor,
			y: coords.y * screenCordinatesFactor,
			w: width * screenCordinatesFactor,
			h: coords.height * screenCordinatesFactor,
			centerY: coords.centerY * screenCordinatesFactor
		});
	}

	return splitBoxes;
};

export const PartituraContext = React.createContext<ActivePartitura | null>(null);
PartituraContext.displayName = "PartituraContext";

export const Partitura = ({ children }: any) => {
	const [error, setError] = useState();
	const [isPartituraLoaded, setIsPartituraLoaded] = useState(false);
	const [partitura, setPartitura] = useState<ActivePartitura | null>(null);

	useEffect(() => {
		fetch("/La Catedral.partitura2")
			.then(res => res.json())
			.then(
				(pascalCased: any) => {
					const result: ActivePartitura = camelcaseKeys(pascalCased, { deep: true });
					setIsPartituraLoaded(true);
					result.name = "La Catedral";
					result.pages.forEach((p: Page, i: number) => {
						p.boxes.map(b => (b.textureCoords = toImageCoordinates(b)));
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
