import { useContext } from "react";
import { PlayerContext } from "./Player";
import { useFullscreenStatus } from "./useFullscreenStatus";

export const PlayerControls = () => {
	const player = useContext(PlayerContext);
	const [isFullscreen, setIsFullscreen] = useFullscreenStatus();
	const toggleFullScreen = () => {
		//fullscreen must be triggered from a user gesture like click event.
		setIsFullscreen(!isFullscreen);
	};

	return (
		<>
			<button tabIndex={-1} onClick={toggleFullScreen}>
				{isFullscreen ? "Exit Fullscreen" : "Fullscreen"}
			</button>
			<button tabIndex={-1} onClick={player?.controller.goToFirstPart}>
				Da Capo
			</button>
		</>
	);
};
