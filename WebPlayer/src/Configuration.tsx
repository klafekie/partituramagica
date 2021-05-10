/* eslint-disable @typescript-eslint/no-unused-vars */
import { createContext, useEffect, useState } from "react";

interface ConfigurationReader {
	read(): ConfigurationData;
}
interface ConfigurationData {
	gesture: GestureConfiguration;
	player: PlayerConfiguration;
	keyboard: KeyboardConfiguration;
	stage: StageConfiguration;
}
interface GestureConfiguration {
	angleTolerance: number; //in degrees,
	swipeDuration: number; //in ms
	borderWidth: number; //in pixel
	pinchZoomFactor: number; //increase to have faster zooming with pinch gesture
	wheelZoomFactor: number; //increase to have faster zooming with mouse wheel
	wheelSpeedFactor: number; //increas to faster change speed with the mouse wheel
	swipeSpeedFactor: number; //increase to faster change speed with swipes
	swipeYPositionFactor: number; //increase if vertical swipe should move vertical position faster
}
interface PlayerConfiguration {
	maximumSpeed: number;
	minimumSpeed: number;
	maximumZoom: number;
	minimumZoom: number;
	resumeDelay: number;
	tickDeltaFactor: number;
	boxDistance: number; //TODO better belongs to stage config
}
interface KeyboardConfiguration {
	initialKeyRepeatDelay: number; //number of ticks to wait on mobile for first key repetition
	keyRepeatDelay: number; //number of ticks to wait on mobile for subsequent key repetitions
	assumeKeyUpDelay: number; //number of millisedoncs to wait before assuming key is up
	puaseKeyCode: string; //key code to use (desktop only)
}
interface StageConfiguration {
	fontSize: number;
	textColor: string;
}

export const ConfigurationContext = createContext<ConfigurationReader>({
	read: () => {
		throw Error("zu früh");
	}
});
ConfigurationContext.displayName = "Configuration";

export const Configuration = ({ children }: any) => {
	const [data, setData] = useState<ConfigurationData | null>(null);

	const read = () => {
		if (data) return data;
		throw Error("sorry keine config da");
	};
	useEffect(() => {
		fetch("/AppConfiguration.json")
			.then(res => res.json())
			.then((data: any) => {
				setData(data);
			});
	}, []);

	//render children only when config is available
	return (
		<ConfigurationContext.Provider value={{ read }}>
			{data ? children : undefined}
		</ConfigurationContext.Provider>
	);
};
