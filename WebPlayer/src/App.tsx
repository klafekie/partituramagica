import { AppProvider } from "@inlet/react-pixi";
import { ErrorBoundary } from "./ErrorBoundary";

import * as PIXI from "pixi.js";
import { Partitura } from "./Partitura";
import { Player } from "./Player";
import { PixiStage } from "./PixiStage";
import { Textures } from "./Textures";

interface Props {
	rootElement: HTMLElement;
}

export const App = ({ rootElement }: Props) => {
	const app = new PIXI.Application({
		resizeTo: rootElement,
		width: rootElement.clientWidth,
		height: rootElement.clientHeight
	});

	return (
		<ErrorBoundary>
			<AppProvider value={app}>
				<Partitura>
					<Textures>
						<Player>
							<PixiStage></PixiStage>
						</Player>
					</Textures>
				</Partitura>
			</AppProvider>
		</ErrorBoundary>
	);
};
