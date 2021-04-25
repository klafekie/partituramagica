/* eslint-disable @typescript-eslint/no-unused-vars */
import { Stage, Text } from "@inlet/react-pixi";
import { TextStyle } from "pixi.js";
import React from "react";
import { useEffect, useState } from "react";
import { Player } from "./Player";

export interface ActivePartitura {
	Name: string;
	Pages: Page[];
}
export interface Page {
	Boxes: Box[];
	ImagePath: string;
}
export interface Box {
	TextureCoords: TextureCoords;
	Coords:Coords;
}
export interface Coords {
	X:number,Y:number,Width:number,Height:number,CenterY:number
}
export interface TextureCoords {
	x: number;
	y: number;
	w: number;
	h: number;
	centerY: number;
}

export const EmptyPartitura: ActivePartitura = { Pages: [], Name:'' };

export const PartituraContext = React.createContext(EmptyPartitura);

export const Partitura = () => {
	const [error, setError] = useState();
	const [isLoaded, setIsLoaded] = useState(false);
	const [partitura, setPartitura] = useState<ActivePartitura|null>(null);

	const toImageCoordinates = (coords: Coords) : TextureCoords => {
		//todo get from image properties
		const screenCordinatesFactor = 600 / 96;

		return {
			x: coords.X * screenCordinatesFactor,
			y: coords.Y * screenCordinatesFactor,
			w: coords.Width * screenCordinatesFactor,
			h: coords.Height * screenCordinatesFactor,
			centerY: coords.CenterY * screenCordinatesFactor,
		};
	};

	useEffect(() => {
		fetch("/La Catedral.partitura2")
			.then((res) => res.json())
			.then(
				(result:ActivePartitura) => {
					setIsLoaded(true);
					result.Name = "La Catedral";
					result.Pages.forEach((p: Page, i: number) => {						
							p.ImagePath = "/" + result.Name + "-00" + i + ".jpg";							
							p.Boxes.map(b => b.TextureCoords = toImageCoordinates(b.Coords) );
						});
					setPartitura(result);
				},
				(error) => {
					setIsLoaded(true);
					setError(error);
				}
			);
	}, []);

	if (error) {
		return <span>Failed to load partitura: {error}</span>;
	} else if (!isLoaded) {
		return <div>Loading...</div>;
	} else if (!partitura) {
		return <div>Couldn'T load partitura</div>
	} else {
	
		return (
			// <PartituraContext.Provider value={partitura}>

			<>
				{/* <Text text="bla" style={new TextStyle({ fontSize: 50, fill:"white"})}>
					</Text> */}

				<Stage 
					options={{
						backgroundColor: 0xffffff,
						resizeTo: window, //todo try without this 
						autoDensity: true //todo try without this
					}}
				>
					<Player activePartitura={partitura} />
				</Stage>
			</>
			// </PartituraContext.Provider>
		);
	}
};
