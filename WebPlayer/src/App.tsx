import { AppProvider } from "@inlet/react-pixi";
import { ErrorBoundary } from "./ErrorBoundary";

import * as PIXI from "pixi.js";
import { Partitura } from "./Partitura";
import { Configuration } from "./Configuration";
import { Player } from "./Player";
import { PixiStage } from "./PixiStage";
import { Textures } from "./Textures";
import { GestureHandler } from "./GestureHandler";
import { KeyboardHandler } from "./KeyboardHandler";
import { PlayerControls } from "./PlayerControls";
import { PlayablePartitura } from "./PlayablePartitura";

interface Props {
	rootElement: HTMLElement;
}

export const App = ({ rootElement }: Props) => {
	const app = new PIXI.Application({
		autoDensity: true,
		autoStart: false,
		width: rootElement.clientWidth,
		height: rootElement.clientHeight - 21 //TODO
	});

	app.ticker.stop();

	return (
		<ErrorBoundary>
			<Configuration>
				<AppProvider value={app}>
					<Partitura>
						<PlayablePartitura>
							<Textures>
								<Player>
									<PlayerControls />
									<KeyboardHandler />
									<GestureHandler>
										<PixiStage></PixiStage>
									</GestureHandler>
								</Player>
							</Textures>
						</PlayablePartitura>
					</Partitura>
				</AppProvider>
			</Configuration>
		</ErrorBoundary>
	);
};
