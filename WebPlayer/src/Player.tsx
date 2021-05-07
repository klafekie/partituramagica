/* eslint-disable @typescript-eslint/no-unused-vars */
import { useDrag } from "react-use-gesture";
import { useTick } from "@inlet/react-pixi";
import { useState } from "react";
import React from "react";

interface PlayerState {
	speed: SpeedState;
	xPos: XPosState;
}
interface SpeedState {
	speed: number;
	manualXPos?: number;
	isFirstManualPosition?: boolean;
}
interface XPosState {
	current: number;
	previous?: number;
}

export const PlayerContext = React.createContext<PlayerState | null>(null);
PlayerContext.displayName = "PlayerContext";

export const Player = ({ children }: any) => {
	const [speed, setSpeed] = useState<SpeedState>({ speed: 6 });
	const [xPos, setXPos] = useState<XPosState>({ current: 0 });

	useTick(delta => {
		if (delta === undefined) delta = 1;
		if (speed.manualXPos !== undefined) {
			if (speed.isFirstManualPosition)
				setXPos({
					current: xPos.current + speed.manualXPos,
					previous: xPos.current
				});
			else {
				let previous = xPos.previous;
				if (previous === undefined) previous = xPos.current;
				setXPos({
					current: previous + speed.manualXPos,
					previous: xPos.previous
				});
			}
		} else setXPos({ current: xPos.current - speed.speed * delta });
	});

	const bind = useDrag(
		({ first, swipe: [swipeX], velocities: [vx], dragging, offset: [offsetX] }) => {
			if (swipeX) setSpeed({ speed: speed.speed - vx });
			else {
				if (dragging)
					setSpeed({
						speed: speed.speed,
						manualXPos: offsetX,
						isFirstManualPosition: first
					});
			}
		}
	);

	return (
		<PlayerContext.Provider value={{ speed, xPos }}>
			<div {...bind()}>{children}</div>
		</PlayerContext.Provider>
	);
};
