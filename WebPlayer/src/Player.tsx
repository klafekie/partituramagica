/* eslint-disable @typescript-eslint/no-unused-vars */
import { useTick } from "@inlet/react-pixi";
import { useContext, useEffect, useState } from "react";
import React from "react";
import { PartituraContext } from "./Partitura";
import { ConfigurationContext } from "./Configuration";
import { PlayablePartituraContext } from "./PlayablePartitura";

interface PlayerState {
	controller: ControllerMethods;

	speed: number;
	offsetY: number;
	zoom: Point;
	pause: PauseState;
	ticker: TickerState;
	meta: MetaState;
}
interface ControllerMethods {
	changeZoomBy(delta: number): void;
	changeZoomXBy(delta: number): void;
	changeZoomYBy(delta: number): void;

	pauseNow(): void;
	resumeNow(): void;
	resumeDelayed(): void;
	togglePause(): void;
	pauseForTicks(ticks: number): void;

	changeXPositionBy(delta: number): void;
	changeYPositionBy(delta: number): void;

	changeSpeedBy(delta: number): void;

	scrollToNavigate(speed: number, reverse: boolean): void;
	cancelScrollNavigation(): void;
	goToFirstPart(): void;
}
interface PauseState {
	isPaused: boolean;
	waitToResume: number;
}
interface Point {
	x: number;
	y: number;
}
interface TickerState {
	currentXPos: number;
}
interface MetaState {
	currentBoxIndex: number;
	positionDescription: string;
}

export const PlayerContext = React.createContext<PlayerState | null>(null);
PlayerContext.displayName = "PlayerContext";

export const Player = ({ children }: any) => {
	const partitura = useContext(PartituraContext);

	const {
		maximumSpeed,
		minimumSpeed,
		maximumZoom,
		minimumZoom,
		resumeDelay,
		tickDeltaFactor,
		boxDistance
	} = useContext(ConfigurationContext).read().player;

	const [speed, setSpeed] = useState<number>(4);
	const [navigationSpeed, setNavigationSpeed] = useState<number | undefined>();
	const [offsetY, setOffsetY] = useState(0);
	const [zoom, setZoom] = useState<Point>({ x: 0.5, y: 0.5 });
	const [pause, setPause] = useState<PauseState>({ isPaused: false, waitToResume: 0 });
	const [tickerState, setTickerState] = useState<TickerState>({ currentXPos: 0 });
	const playablePartitura = useContext(PlayablePartituraContext);

	const [metaState, setMetaState] = useState<MetaState>({
		currentBoxIndex: 0,
		positionDescription: ""
	});

	const togglePause = () => {
		console.log("toggle isPaused to " + !pause.isPaused);
		setPause({ isPaused: !pause.isPaused, waitToResume: 0 });
	};
	const pauseNow = () => {
		console.log("pause now");
		setPause({ isPaused: true, waitToResume: 0 });
	};
	const resumeNow = () => {
		console.log("resume now");
		setPause({ isPaused: false, waitToResume: 0 });
	};
	const resumeDelayed = () => {
		console.log("resumeDelayed");
		setPause({ isPaused: true, waitToResume: resumeDelay });
	};
	const pauseForTicks = (tickCount: number) => {
		console.log("resumeDelayed");
		setPause({ isPaused: true, waitToResume: tickCount });
	};

	const ensureZoomBounds = (zoom: Point) => {
		if (zoom.x < minimumZoom) zoom.x = minimumZoom;
		if (zoom.x > maximumZoom) zoom.x = maximumZoom;
		if (zoom.y < minimumZoom) zoom.y = minimumZoom;
		if (zoom.y > maximumZoom) zoom.y = maximumZoom;
		return zoom;
	};
	const changeZoomBy = (delta: number): void => {
		if (delta === 0) return;
		zoom.x += delta;
		zoom.y += delta;
		console.log("changing zoom by " + delta + " to ", zoom);
		setZoom({ ...ensureZoomBounds(zoom) });
	};
	const changeZoomXBy = (delta: number): void => {
		if (delta === 0) return;
		zoom.x += delta;
		console.log("changing zoom to ", zoom);
		setZoom({ ...ensureZoomBounds(zoom) });
	};
	const changeZoomYBy = (delta: number): void => {
		//TODO better preserve x position while zooming
		if (delta === 0) return;
		zoom.y += delta;
		console.log("changing zoom to ", zoom);
		setZoom({ ...ensureZoomBounds(zoom) });
	};

	const changeYPositionBy = (delta: number): void => {
		if (delta === 0) return;
		console.log("changing vertical position by " + delta);
		setOffsetY(offsetY + delta);
	};

	const changeXPositionBy = (delta: number): void => {
		if (delta === 0) return;

		let currentXPos = tickerState.currentXPos + delta / zoom.x;
		if (currentXPos !== tickerState.currentXPos) setTickerState({ ...tickerState, currentXPos });
	};

	const changeSpeedBy = (deltaPercent: number): void => {
		if (deltaPercent === 0) return;
		const delta = (deltaPercent * speed) / 100;
		const newSpeed = Math.min(Math.max(speed - delta, minimumSpeed), maximumSpeed);
		if (newSpeed === speed) return;

		setSpeed(newSpeed);
		console.log("changing speed by " + deltaPercent + " to " + speed);
	};

	const goToFirstPart = () => {
		pauseNow();
		setTickerState({ ...tickerState, currentXPos: 0 });
		//todo either always use ticks or timeout for resume delayed
		setTimeout(resumeNow, 600);
	};

	const scrollToNavigate = (speed: number, reverse: boolean): void => {
		if (!speed) return;
		if (speed < 0) speed = -speed;

		//todo avoid magic numbers. introduce constants/configuration
		let newSpeed = Math.min(Math.max(speed, minimumSpeed), maximumSpeed);
		if (reverse) newSpeed = -newSpeed;
		if (newSpeed === navigationSpeed) return;
		console.log("navigating with speed " + newSpeed);
		setNavigationSpeed(newSpeed);
	};

	const cancelScrollNavigation = () => {
		if (navigationSpeed) {
			console.log("navigation scroll cancelled");
			setNavigationSpeed(undefined);
		}
	};

	useTick(delta => {
		if (!playablePartitura) return;
		const { totalLength } = playablePartitura;
		const setNextXPos = (newSpeed: number) => {
			let { currentXPos } = tickerState;
			if (delta === undefined) delta = 0;
			const difference = newSpeed + delta * tickDeltaFactor;
			currentXPos = currentXPos - difference;

			//use this limit also when dragging horizontally?
			if (currentXPos > 0) currentXPos = 0;
			if (currentXPos < -totalLength) currentXPos = -totalLength;

			if (currentXPos !== tickerState.currentXPos) setTickerState({ ...tickerState, currentXPos });
		};

		if (pause.waitToResume > 0) {
			pause.waitToResume -= 1;
			if (pause.waitToResume === 0) {
				pause.isPaused = false;
			}
			setPause(pause);
		}

		if (navigationSpeed !== undefined) {
			setNextXPos(navigationSpeed);
		} else if (!pause.isPaused) {
			setNextXPos(speed);
		}
	});

	//set positionDescription
	//TODO move to new navigator component?
	useEffect(() => {
		if (!playablePartitura) return;
		const { boxes } = playablePartitura;

		for (let bI = 0; bI < boxes.length; bI++) {
			const box = boxes[bI];
			if (box.x + box.w >= -tickerState.currentXPos - boxDistance) {
				if (metaState.currentBoxIndex !== bI || !metaState.positionDescription) {
					const currentPartName = String.fromCharCode("A".charCodeAt(0) + box.currentPartIndex);
					setMetaState({
						currentBoxIndex: bI,
						positionDescription: currentPartName + " / " + box.pageIndex + " / " + box.lineIndex
					});
				}
				return;
			}
		}
	}, [
		boxDistance,
		metaState.currentBoxIndex,
		metaState.positionDescription,
		playablePartitura,
		tickerState.currentXPos
	]);

	if (!partitura) return <div>...loading</div>;

	return (
		<PlayerContext.Provider
			value={{
				controller: {
					changeZoomBy,
					changeZoomXBy,
					changeZoomYBy,

					pauseNow,
					resumeNow,
					resumeDelayed,
					togglePause,
					pauseForTicks,

					changeYPositionBy,
					changeXPositionBy,
					changeSpeedBy,

					goToFirstPart,
					scrollToNavigate,
					cancelScrollNavigation
				},

				speed,
				offsetY,
				zoom,
				pause,
				ticker: tickerState,
				meta: metaState
			}}>
			{children}
		</PlayerContext.Provider>
	);
};
