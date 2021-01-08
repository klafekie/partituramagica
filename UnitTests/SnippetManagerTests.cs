#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;

#endregion

namespace PartituraMagica.Test
{
	[TestFixture]
	public class SnippetManagerTests
	{
		[SetUp]
		public void Setup()
		{
			_mockRepository = new MockRepository(MockBehavior.Default);
			_positionHolder = new PlayerPositionHolder();
			_speedManager = _mockRepository.Create<ISpeedManager>();
			_memento = new SnippetMemento();
			_playerView = _mockRepository.Create<IPlayerView>();
		}

		[TearDown]
		public void TearDown()
		{
			_mockRepository.Verify();
		}

		private MockRepository _mockRepository;
		private PlayerPositionHolder _positionHolder;
		private Mock<ISpeedManager> _speedManager;
		private SnippetMemento _memento;
		private Mock<IPlayerView> _playerView;

		[Test]
		public void JustCreateAndFinish()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			dut.Init(_memento, _playerView.Object);

			dut.HasChanges.Should().BeFalse();
			dut.ToMemento().Snippets.Should().BeNullOrEmpty();

			dut.Finish();

			dut.HasChanges.Should().BeFalse();
			dut.ToMemento().Snippets.Should().BeNullOrEmpty();
		}

		[Test]
		public void RecordOneSnippet()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			dut.Init(_memento, _playerView.Object);

			_playerView.Verify(view => view.UpdateAvailableSnippets(0));

			{
				_positionHolder.XPos = 10;
				dut.StartRecording();
				_positionHolder.XPos = 30;
				dut.StopRecording();
			}

			_playerView.Verify(view => view.UpdateAvailableSnippets(1));

			{
				dut.Finish();
			}

			dut.HasChanges.Should().BeTrue();
			dut.ToMemento().Snippets.Count.Should().Be(1);
			dut.ToMemento().Snippets.First().Key.Should().Be(10);
			dut.ToMemento().Snippets.First().Value.Should().Be(30);
		}

		[Test]
		public void RepeatOneSnippet()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			_memento.Snippets = new List<KeyValuePair<double, double>>
			{
				new KeyValuePair<double, double>(1, 3)
			};
			dut.Init(_memento, _playerView.Object);
			{
				dut.RepeatSnippet(0);
			}

			ScrollForward();
			ScrollForward();
			ScrollForward();
			ScrollForward();
			ScrollForward();
			ScrollForward();
			ScrollForward();
			ScrollForward();
			ScrollForward();
			ScrollForward(true);

			void ScrollForward(bool isLast = false)
			{
				Action afterPauseAction = null;

				_positionHolder.XPos.Should().Be(1);
				_playerView.Setup(view => view.ExecuteDelayed(It.IsAny<int>(), It.IsAny<Action>()))
					.Callback((int ms, Action action) => afterPauseAction = action);
				{
					_positionHolder.ScrollForwardBy(1);
				}
				_positionHolder.XPos.Should().Be(2); //scrolling continues

				{
					_positionHolder.ScrollForwardBy(1);
				}
				if (isLast)
				{
					_positionHolder.XPos.Should().Be(3); //pause after last iteration
					_speedManager.VerifySet(s => s.IsPaused = true);
				}
				else
				{
					_positionHolder.XPos.Should().Be(3); //end of snippet reached
					afterPauseAction.Should().NotBeNull();
					afterPauseAction();
				}
			}
		}


		[Test]
		public void RepeatTwoSnippets()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			_memento.Snippets = new List<KeyValuePair<double, double>>
			{
				new KeyValuePair<double, double>(1, 3),
				new KeyValuePair<double, double>(5, 7)
			};
			dut.Init(_memento, _playerView.Object);
			{
				dut.RepeatAllSnippets();
			}

			ScrollForward(1);
			ScrollForward(1);
			ScrollForward(1);
			ScrollForward(5);
			ScrollForward(5);
			ScrollForward(5, true);

			void ScrollForward(int startPos, bool isLast = false)
			{
				Action afterPauseAction = null;

				_positionHolder.XPos.Should().Be(startPos);
				_playerView.Setup(view => view.ExecuteDelayed(It.IsAny<int>(), It.IsAny<Action>()))
					.Callback((int ms, Action action) => afterPauseAction = action);
				{
					_positionHolder.ScrollForwardBy(1);
				}
				_positionHolder.XPos.Should().Be(startPos + 1); //scrolling continues

				{
					_positionHolder.ScrollForwardBy(1);
				}
				if (isLast)
				{
					_positionHolder.XPos.Should().Be(startPos + 2); //pause after last iteration
					_speedManager.VerifySet(s => s.IsPaused = true);
				}
				else
				{
					_positionHolder.XPos.Should().Be(startPos + 2); //end of snippet reached
					afterPauseAction.Should().NotBeNull();
					afterPauseAction();
				}
			}
		}

		[Test]
		public void RepeatTwoOverlappingSnippets()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			_memento.Snippets = new List<KeyValuePair<double, double>>
			{
				new KeyValuePair<double, double>(1, 3),
				new KeyValuePair<double, double>(2, 4)
			};
			dut.Init(_memento, _playerView.Object);
			{
				dut.RepeatAllSnippets();
			}

			ScrollForward(1);
			ScrollForward(1);
			ScrollForward(1);
			ScrollForward(2);
			ScrollForward(2);
			ScrollForward(2, true);

			void ScrollForward(int startPos, bool isLast = false)
			{
				Action afterPauseAction = null;

				_positionHolder.XPos.Should().Be(startPos);
				_playerView.Setup(view => view.ExecuteDelayed(It.IsAny<int>(), It.IsAny<Action>()))
					.Callback((int ms, Action action) => afterPauseAction = action);
				{
					_positionHolder.ScrollForwardBy(1);
				}
				_positionHolder.XPos.Should().Be(startPos + 1); //scrolling continues

				{
					_positionHolder.ScrollForwardBy(1);
				}
				if (isLast)
				{
					_positionHolder.XPos.Should().Be(startPos + 2); //pause after last iteration
					_speedManager.VerifySet(s => s.IsPaused = true);
				}
				else
				{
					_positionHolder.XPos.Should().Be(startPos + 2); //end of snippet reached
					afterPauseAction.Should().NotBeNull();
					afterPauseAction();
				}
			}
		}

		[Test]
		public void CloseWithoutStopRecording()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			dut.Init(_memento, _playerView.Object);

			_positionHolder.XPos = 10;
			dut.StartRecording();

			_positionHolder.XPos = 30;

			//stop recording ommitted

			dut.Finish();

			dut.HasChanges.Should().BeTrue();
			dut.ToMemento().Snippets.Count.Should().Be(1);
			dut.ToMemento().Snippets.First().Key.Should().Be(10);
			dut.ToMemento().Snippets.First().Value.Should().Be(30);
		}

		[Test]
		public void SnippetTooSmallToRecord()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			dut.Init(_memento, _playerView.Object);

			_positionHolder.XPos = 1;
			dut.StartRecording();

			_positionHolder.XPos = 2;

			dut.StopRecording();

			dut.Finish();

			dut.HasChanges.Should().BeFalse();
			dut.ToMemento().Snippets.Count.Should().Be(0);
		}

		[Test]
		public void SnippedAlreadyExists()
		{
			var dut = new SnippetManager(_positionHolder, _speedManager.Object);
			_memento.Snippets = new List<KeyValuePair<double, double>>
			{
				new KeyValuePair<double, double>(11, 21)
			};

			dut.Init(_memento, _playerView.Object);

			_positionHolder.XPos = 11;
			dut.StartRecording();

			_positionHolder.XPos = 22;

			dut.StopRecording();

			dut.Finish();

			dut.HasChanges.Should().BeTrue();
			dut.ToMemento().Snippets.Count.Should().Be(1);
			dut.ToMemento().Snippets.First().Key.Should().Be(11);
			dut.ToMemento().Snippets.First().Value.Should().Be(22, "extend existing snippet if XPos is bigger");
		}
	}
}