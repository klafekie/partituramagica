import { AppProvider } from "@inlet/react-pixi";
import { ErrorBoundary } from "./ErrorBoundary";
import { Partitura } from "./Partitura";
import * as PIXI from "pixi.js";

export const App = ({window}:any) => {
	const app = new PIXI.Application({
		resizeTo: window,		
		autoDensity: true		//TODO is this really good??
	});
		
	return (
		<ErrorBoundary>
		<AppProvider value={app}>			
				<Partitura></Partitura>			
		</AppProvider>
		</ErrorBoundary>
	);
};
