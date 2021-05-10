/* eslint-disable @typescript-eslint/no-unused-vars */
import { useGesture } from "react-use-gesture";
import { useContext, useRef } from "react";
import { PlayerContext } from "./Player";
import { ConfigurationContext } from "./Configuration";
import { useApp } from "@inlet/react-pixi";

export const GestureHandler = ({ children }: any) => {
	const player = useContext(PlayerContext);
	const { screen } = useApp();
	const configuration = useContext(ConfigurationContext);

	const {
		player: { maximumSpeed },
		gesture: {
			angleTolerance,
			swipeDuration,
			borderWidth,
			pinchZoomFactor,
			wheelZoomFactor,
			wheelSpeedFactor,
			swipeSpeedFactor,
			swipeYPositionFactor
		}
	} = configuration.read();

	const angleIsNear = (angle: number, degree: number) => {
		angle += 360; //eliminate special case when angle is near 0 or 359...
		degree += 360;
		return angle >= degree - angleTolerance && angle < degree + angleTolerance;
	};

	const gestureTarget = useRef(null);

	useGesture(
		{
			onDrag: dragState => {
				if (!player) return;
				const {
					last,
					swipe: [swipeX, swipeY],
					tap,
					velocities: [vx, vy],
					elapsedTime,
					delta: [deltaX],
					xy: [x]
				} = dragState;

				if (swipeX !== 0) {
					player.controller.changeSpeedBy(vx * swipeSpeedFactor);
				} else if (swipeY !== 0) {
					player.controller.changeYPositionBy(vy * swipeYPositionFactor);
				} else if (tap) {
					//TODO navigate to part if tapped at the top area
					//da capo for double tap
					//navigate to previous/next page if tapped at left/right margin
					player.controller.togglePause();
				} else if (elapsedTime > swipeDuration) {
					if (last) {
						player.controller.cancelScrollNavigation();
						player.controller.resumeDelayed();
						return;
					} else if (!player.pause.isPaused) {
						player.controller.pauseNow();
					}
					const leftEdge = borderWidth;
					const rightEdge = screen.width - borderWidth;
					if (x <= leftEdge) {
						const speed = Math.round(((leftEdge - x) / borderWidth) * maximumSpeed);
						player.controller.scrollToNavigate(speed, false);
					} else if (x >= rightEdge) {
						const speed = Math.round(((x - rightEdge) / borderWidth) * maximumSpeed);
						player.controller.scrollToNavigate(speed, true);
					} else {
						player.controller.cancelScrollNavigation();
						player.controller.changeXPositionBy(deltaX);
					}
				}
			},
			onPinch: state => {
				if (!player) return;
				let {
					first,
					last,
					da: [, angle],
					delta: [distanceDelta],
					event,
					shiftKey
				} = state;

				//control + mousewheel leads to a pinch event.
				if (event.type === "wheel") {
					if (shiftKey) {
						player.controller.changeZoomXBy(distanceDelta * wheelZoomFactor);
					} else {
						player.controller.changeZoomBy(distanceDelta * wheelZoomFactor);
					}
				} else {
					angle = Math.abs(angle); //depending on which finger is set first the angle might be negative

					if (angleIsNear(angle, 0) || angleIsNear(angle, 180)) {
						player.controller.changeZoomYBy(distanceDelta * pinchZoomFactor);
					} else if (angleIsNear(angle, 90) || angleIsNear(angle, 270)) {
						player.controller.changeZoomXBy(distanceDelta * pinchZoomFactor);
					} else {
						player.controller.changeZoomBy(distanceDelta * pinchZoomFactor);
					}
				}

				if (first) {
					player.controller.pauseNow();
				}
				if (last) {
					player.controller.resumeDelayed();
				}
			},
			onWheel: state => {
				//delta y is normally 100 or -100
				if (state.wheeling && player)
					player?.controller.changeSpeedBy(state.delta[1] * wheelSpeedFactor);
			}
		},
		{
			domTarget: gestureTarget,
			eventOptions: { passive: false },
			drag: { filterTaps: true, swipeDuration }
		}
	);

	return (
		<div ref={gestureTarget} style={{ touchAction: "none" }}>
			{children}
		</div>
	);
};
