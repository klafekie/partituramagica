/* eslint-disable @typescript-eslint/no-unused-vars */
import { useApp } from "@inlet/react-pixi";
import { useContext, useEffect, useState } from "react";
import { createContext } from "react";
import { PartituraContext } from "./Partitura";
import { ConfigurationContext } from "./Configuration";

interface PlayablePartituraState {
	boxes: BoxState[];
	totalLength: number;
}
interface BoxState {
	pageIndex: number;
	lineIndex: number;
	splitterIndex: number;
	currentPartIndex: number;

	x: number;
	y: number;
	w: number;
	h: number;
}

export const PlayablePartituraContext = createContext<PlayablePartituraState | null>(null);
PlayablePartituraContext.displayName = "Playable Partitura";

//responsible for "expanding/flattening" partitura based on part structure to make it easer to display and navigate
export const PlayablePartitura = ({ children }: any) => {
	const partitura = useContext(PartituraContext);

	const { boxDistance } = useContext(ConfigurationContext).read().player;

	const [boxes, setBoxes] = useState<BoxState[]>([]);
	const [totalLength, setTotalLength] = useState(0);

	//calculate boxes
	useEffect(() => {
		if (!partitura) return;

		const addPart = (
			result: BoxState[],
			rightMost: number,
			partIndex: number,
			partEntry: { p: number; b: number; t: number }
		): number => {
			if (!partitura) return rightMost;
			let startAt = { ...partEntry };
			for (let pI = startAt.p; pI < partitura.pages.length; pI++) {
				const page = partitura.pages[pI];

				for (let bI = startAt.b; bI < page.boxes.length; bI++) {
					const box = page.boxes[bI];

					for (let tI = startAt.t; tI < box.textureCoords.length; tI++) {
						const t = box.textureCoords[tI];

						if (t.currentPartIndex !== partIndex) return rightMost;

						startAt.b = 0;
						startAt.t = 0;

						const { centerY, w, h } = t;
						const yPos = -centerY;

						result.push({
							pageIndex: pI,
							lineIndex: bI,
							splitterIndex: tI,
							currentPartIndex: t.currentPartIndex || 0,
							x: rightMost,
							y: yPos,
							w: w,
							h: h
						});
						rightMost += w + boxDistance;
					}
				}
			}
			return rightMost;
		};

		let partEntries: { p: number; b: number; t: number }[] = [{ p: 0, b: 0, t: 0 }];
		let currentPartIndex = 0;
		partitura.pages.forEach((p, pI) => {
			p.boxes.forEach((b, bI) => {
				let { labelIds } = b;
				b.textureCoords.forEach((t, tI) => {
					if (labelIds.includes(t.id)) {
						currentPartIndex += 1;
						partEntries.push({ p: pI, b: bI, t: tI });
					}
					t.currentPartIndex = currentPartIndex;
				});
			});
		});

		let rightMost = 0;
		let result: BoxState[] = [];

		let { partPattern } = partitura;
		partPattern = partPattern || "A";
		for (let patternIndex = 0; patternIndex < partPattern.length; patternIndex++) {
			const partIndex = partPattern.charCodeAt(patternIndex) - "A".charCodeAt(0);
			const partEntry = partEntries[partIndex];

			rightMost = addPart(result, rightMost, partIndex, partEntry);
		}

		setBoxes(result);
		setTotalLength(rightMost);
	}, [boxDistance, partitura]);

	if (!partitura) return <div>...loading</div>;

	return (
		<PlayablePartituraContext.Provider
			value={{
				boxes,
				totalLength
			}}>
			{children}
		</PlayablePartituraContext.Provider>
	);
};
