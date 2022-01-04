/* eslint-disable @typescript-eslint/no-unused-vars */
import { useContext, useEffect, useRef, useState } from "react";
import { PlayerContext } from "./Player";
import { MobileView, BrowserView } from "react-device-detect";
import { ConfigurationContext } from "./Configuration";

export const KeyboardHandler = () => {
	const player = useContext(PlayerContext);
	const inputElement = useRef<HTMLInputElement>(null);
	const [isFirstKeyDownMobile, setIsFirstKeyDownMobile] = useState(true);

	const { initialKeyRepeatDelay, keyRepeatDelay, assumeKeyUpDelay, puaseKeyCode } =
		useContext(ConfigurationContext).read().keyboard;

	const onKeyDownMobile = (e: any) => {
		//on mobile code is always the same. Also keyUp event is not working as expected.
		//so we just rely on the repeat function of footswitch "keyboard"
		//the pause between first and second character is bigger than for the remaining repetitions.
		if (isFirstKeyDownMobile) player?.controller.pauseForTicks(initialKeyRepeatDelay);
		else player?.controller.pauseForTicks(keyRepeatDelay);

		setIsFirstKeyDownMobile(false);
		setTimeout(() => setIsFirstKeyDownMobile(true), assumeKeyUpDelay);
		e.preventDefault();
	};
	const onKeyDown = (e: any) => {
		if (e.code === puaseKeyCode) {
			player?.controller.pauseNow();
			e.preventDefault();
		}
	};
	const onKeyUp = (e: any) => {
		if (e.code === puaseKeyCode) {
			player?.controller.resumeNow();
			e.preventDefault();
		}
	};

	useEffect(() => {
		if (inputElement && inputElement.current) {
			inputElement.current.focus({ preventScroll: true });
		}
	});

	return (
		<>
			<MobileView renderWithFragment={true}>
				<input
					type="text"
					ref={inputElement}
					onKeyDown={onKeyDownMobile}
					tabIndex={-1}
					readOnly={true}
					style={{ left: -1000, right: -1000 }}></input>
			</MobileView>
			<BrowserView renderWithFragment={true}>
				<input
					type="text"
					ref={inputElement}
					onKeyDown={onKeyDown}
					onKeyUp={onKeyUp}
					tabIndex={-1}
					style={{ position: "absolute", left: -1000, right: -1000 }}></input>
			</BrowserView>
		</>
	);
};
