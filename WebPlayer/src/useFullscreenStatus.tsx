import { useLayoutEffect, useState } from "react";

export function useFullscreenStatus(): [boolean, (value: boolean) => void] {
	const [isFullscreenState, setIsFullscreenState] = useState(document.fullscreenElement != null);

	const setFullscreen = (value: boolean) => {
		if (value === false && document.fullscreenElement) {
			document
				.exitFullscreen()
				.then(() => setIsFullscreenState(document.fullscreenElement != null))
				.catch(() => setIsFullscreenState(true));
			return;
		}

		if (value === true) {
			document.documentElement
				.requestFullscreen()
				.then(() => setIsFullscreenState(document.fullscreenElement != null))
				.catch(() => setIsFullscreenState(false));
		}
	};

	useLayoutEffect(() => {
		document.onfullscreenchange = () => setIsFullscreenState(document.fullscreenElement != null);

		return () => {
			document.onfullscreenchange = null;
			return;
		};
	});

	return [isFullscreenState, setFullscreen];
}
