using UnityEngine;
using System.Text;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2015 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro Movie Capture/GUI")]
public class AVProMovieCaptureGUI : MonoBehaviour 
{
	public AVProMovieCaptureBase _movieCapture;
	public bool _showUI = true;
	public bool _whenRecordingAutoHideUI = true;
	public GUISkin _guiSkin;
	
	// GUI
	private int _shownSection = -1;
	private string[] _videoCodecNames;
	private string[] _audioCodecNames;
	private bool[] _videoCodecConfigurable;
	private bool[] _audioCodecConfigurable;
	private string[] _audioDeviceNames;
	private string[] _downScales;
	private string[] _frameRates;
	private int _downScaleIndex;
	private int _frameRateIndex;
	private Vector2 _videoPos = Vector2.zero;
	private Vector2 _audioPos = Vector2.zero;
	private Vector2 _audioCodecPos = Vector2.zero;

	// Status
	private long _lastFileSize;
	private uint _lastEncodedMinutes;
	private uint _lastEncodedSeconds;

	void Start()
	{
		CreateGUI();
	}
		
	private void CreateGUI()
	{
		_downScales = new string[6];
		_downScales[0] = "Original";
		_downScales[1] = "Half";
		_downScales[2] = "Quarter";
		_downScales[3] = "Eighth";
		_downScales[4] = "Sixteenth";
		_downScales[5] = "Specific";
		switch (_movieCapture._downScale)
		{
		default:
		case AVProMovieCaptureBase.DownScale.Original:
			_downScaleIndex = 0;	
			break;
		case AVProMovieCaptureBase.DownScale.Half:
			_downScaleIndex = 1;	
			break;
		case AVProMovieCaptureBase.DownScale.Quarter:
			_downScaleIndex = 2;	
			break;
		case AVProMovieCaptureBase.DownScale.Eighth:
			_downScaleIndex = 3;	
			break;
		case AVProMovieCaptureBase.DownScale.Sixteenth:
			_downScaleIndex = 4;	
			break;
		case AVProMovieCaptureBase.DownScale.Specific:
			_downScaleIndex = 5;
			break;
		}
		
		_frameRates = new string[6];
		_frameRates[0] = "15";
		_frameRates[1] = "24";
		_frameRates[2] = "25";
		_frameRates[3] = "30";
		_frameRates[4] = "50";
		_frameRates[5] = "60";
		switch (_movieCapture._frameRate)
		{
		default:
		case AVProMovieCaptureBase.FrameRate.Fifteen:
			_frameRateIndex = 0;
			break;
		case AVProMovieCaptureBase.FrameRate.TwentyFour:
			_frameRateIndex = 1;
			break;
		case AVProMovieCaptureBase.FrameRate.TwentyFive:
			_frameRateIndex = 2;
			break;
		case AVProMovieCaptureBase.FrameRate.Thirty:
			_frameRateIndex = 3;
			break;
		case AVProMovieCaptureBase.FrameRate.Fifty:
			_frameRateIndex = 4;
			break;
		case AVProMovieCaptureBase.FrameRate.Sixty:
			_frameRateIndex = 5;
			break;
		}

		int numVideoCodecs = AVProMovieCapturePlugin.GetNumAVIVideoCodecs();
		if (numVideoCodecs > 0)
		{
			_videoCodecNames = new string[numVideoCodecs+1];
			_videoCodecNames[0] = "Uncompressed";
			_videoCodecConfigurable = new bool[numVideoCodecs];
			for (int i = 0; i < numVideoCodecs; i++)
			{
				_videoCodecNames[i+1] = AVProMovieCapturePlugin.GetAVIVideoCodecName(i);
				_videoCodecConfigurable[i] = AVProMovieCapturePlugin.IsConfigureVideoCodecSupported(i);
			}
		}

		int numAudioDevices = AVProMovieCapturePlugin.GetNumAVIAudioInputDevices();
		if (numAudioDevices > 0)
		{
			_audioDeviceNames = new string[numAudioDevices+1];
			_audioDeviceNames[0] = "Unity";
			for (int i = 0; i < numAudioDevices; i++)
			{
				_audioDeviceNames[i + 1] = AVProMovieCapturePlugin.GetAVIAudioInputDeviceName(i);
			}
		}

		int numAudioCodecs = AVProMovieCapturePlugin.GetNumAVIAudioCodecs();
		if (numAudioCodecs > 0)
		{
			_audioCodecNames = new string[numAudioCodecs+1];
			_audioCodecNames[0] = "Uncompressed";
			_audioCodecConfigurable = new bool[numAudioCodecs];
			for (int i = 0; i < numAudioCodecs; i++)
			{
				_audioCodecNames[i + 1] = AVProMovieCapturePlugin.GetAVIAudioCodecName(i);
				_audioCodecConfigurable[i] = AVProMovieCapturePlugin.IsConfigureAudioCodecSupported(i);
			}
		}		

		_movieCapture.SelectCodec(false);
		_movieCapture.SelectAudioCodec(false);
		_movieCapture.SelectAudioDevice(false);
	}

	void OnGUI()
	{
		GUI.skin = _guiSkin;
		GUI.depth = -10;
		
		if (_showUI)
			GUILayout.Window(4, new Rect(0, 0, 450, 256), MyWindow, "AVPro Movie Capture UI");
	}

	void MyWindow(int id)
	{
		if (_movieCapture.IsCapturing())
		{
			GUI_RecordingStatus();
			return;
		}

		GUILayout.BeginVertical();
		
		if (_movieCapture != null)
		{		
			GUILayout.Label("Resolution:");
			GUILayout.BeginHorizontal();
			_downScaleIndex = GUILayout.SelectionGrid(_downScaleIndex, _downScales, _downScales.Length);
			switch (_downScaleIndex)
			{
				case 0:
					_movieCapture._downScale = AVProMovieCaptureBase.DownScale.Original;
					break;
				case 1:
					_movieCapture._downScale = AVProMovieCaptureBase.DownScale.Half;
					break;
				case 2:
					_movieCapture._downScale = AVProMovieCaptureBase.DownScale.Quarter;
					break;
				case 3:
					_movieCapture._downScale = AVProMovieCaptureBase.DownScale.Eighth;
					break;
				case 4:
					_movieCapture._downScale = AVProMovieCaptureBase.DownScale.Sixteenth;
					break;
				case 5:
					_movieCapture._downScale = AVProMovieCaptureBase.DownScale.Specific;
					break;
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal(GUILayout.Width(256));
			if (_movieCapture._downScale == AVProMovieCaptureBase.DownScale.Specific)
			{
				string maxWidthString = GUILayout.TextField(Mathf.FloorToInt(_movieCapture._maxVideoSize.x).ToString(), 4);
				int maxWidth = 0;
				if (int.TryParse(maxWidthString, out maxWidth))
				{
					_movieCapture._maxVideoSize.x = Mathf.Clamp(maxWidth, 0, 4096);
				}
				
				GUILayout.Label("x", GUILayout.Width(20));
					
				string maxHeightString = GUILayout.TextField(Mathf.FloorToInt(_movieCapture._maxVideoSize.y).ToString(), 4);
				int maxHeight = 0;
				if (int.TryParse(maxHeightString, out maxHeight))
				{
					_movieCapture._maxVideoSize.y = Mathf.Clamp(maxHeight, 0, 4096);
				}
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Frame Rate:");
			_frameRateIndex = GUILayout.SelectionGrid(_frameRateIndex, _frameRates, _frameRates.Length);
			switch (_frameRateIndex)
			{
				case 0:
					_movieCapture._frameRate = AVProMovieCaptureBase.FrameRate.Fifteen;
					break;
				case 1:
					_movieCapture._frameRate = AVProMovieCaptureBase.FrameRate.TwentyFour;
					break;
				case 2:
					_movieCapture._frameRate = AVProMovieCaptureBase.FrameRate.TwentyFive;
					break;
				case 3:
					_movieCapture._frameRate = AVProMovieCaptureBase.FrameRate.Thirty;
					break;
				case 4:
					_movieCapture._frameRate = AVProMovieCaptureBase.FrameRate.Fifty;
					break;
				case 5:
					_movieCapture._frameRate = AVProMovieCaptureBase.FrameRate.Sixty;
					break;
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(16f);
			
			_movieCapture._isRealTime = GUILayout.Toggle(_movieCapture._isRealTime, "RealTime");
			
			GUILayout.Space(16f);
			
			
			
			
			// Video Codec
			GUILayout.BeginHorizontal();
			if (_shownSection != 0)
			{
				if (GUILayout.Button("+", GUILayout.Width(24)))
				{
					_shownSection = 0;
				}
			}
			else
			{
				if (GUILayout.Button("-", GUILayout.Width(24)))
				{
					_shownSection = -1;
				}
			}
			GUILayout.Label("Using Video Codec: " + _movieCapture._codecName);
			if (_movieCapture._codecIndex >= 0 && _videoCodecConfigurable[_movieCapture._codecIndex])
			{	
				GUILayout.Space(16f);
				if (GUILayout.Button("Configure Codec"))
				{
					AVProMovieCapturePlugin.ConfigureVideoCodec(_movieCapture._codecIndex);
				}
			}			
			GUILayout.EndHorizontal();
				
			if (_videoCodecNames != null && _shownSection == 0)
			{
				GUILayout.Label("Select Video Codec:");
				_videoPos = GUILayout.BeginScrollView(_videoPos, GUILayout.Height(100));
				int newCodecIndex = GUILayout.SelectionGrid(-1, _videoCodecNames, 1) - 1;
				GUILayout.EndScrollView();
				
				if (newCodecIndex >= -1)
				{
					_movieCapture._codecIndex = newCodecIndex;
					if (_movieCapture._codecIndex >= 0)
						_movieCapture._codecName = _videoCodecNames[_movieCapture._codecIndex + 1];
					else
						_movieCapture._codecName = "Uncompressed";
					
					_shownSection = -1;
				}
					
				GUILayout.Space(16f);
			}
			
			
			_movieCapture._noAudio = !GUILayout.Toggle(!_movieCapture._noAudio, "Record Audio");
			GUI.enabled = !_movieCapture._noAudio;
			
			// Audio Device
			GUILayout.BeginHorizontal();
			if (_shownSection != 1)
			{
				if (GUILayout.Button("+", GUILayout.Width(24)))
				{
					_shownSection = 1;
				}
			}
			else
			{
				if (GUILayout.Button("-", GUILayout.Width(24)))
				{
					_shownSection = -1;
				}
			}			
			GUILayout.Label("Using Audio Source: " + _movieCapture._audioDeviceName);
			GUILayout.EndHorizontal();
			if (_audioDeviceNames != null && _shownSection == 1)
			{
				GUILayout.Label("Select Audio Source:");
				_audioPos = GUILayout.BeginScrollView(_audioPos, GUILayout.Height(100));
				int newAudioIndex = GUILayout.SelectionGrid(-1, _audioDeviceNames, 1) - 1;
				GUILayout.EndScrollView();
				
				if (newAudioIndex >= -1)
				{
					_movieCapture._audioDeviceIndex = newAudioIndex;
					if (_movieCapture._audioDeviceIndex >= 0)
						_movieCapture._audioDeviceName = _audioDeviceNames[_movieCapture._audioDeviceIndex + 1];
					else
						_movieCapture._audioDeviceName = "Unity";
					
					_shownSection = -1;
				}

				GUILayout.Space(16f);
			}
			
			
			
			// Audio Codec
			GUILayout.BeginHorizontal();
			if (_shownSection != 2)
			{
				if (GUILayout.Button("+", GUILayout.Width(24)))
				{
					_shownSection = 2;
				}
			}
			else
			{
				if (GUILayout.Button("-", GUILayout.Width(24)))
				{
					_shownSection = -1;
				}
			}
			GUILayout.Label("Using Audio Codec: " + _movieCapture._audioCodecName);
			if (_movieCapture._audioCodecIndex >= 0 && _audioCodecConfigurable[_movieCapture._audioCodecIndex])
			{	
				GUILayout.Space(16f);
				if (GUILayout.Button("Configure Codec"))
				{
					AVProMovieCapturePlugin.ConfigureAudioCodec(_movieCapture._audioCodecIndex);
				}
			}			
			GUILayout.EndHorizontal();
				
			if (_audioCodecNames != null && _shownSection == 2)
			{
				GUILayout.Label("Select Audio Codec:");
				_audioCodecPos = GUILayout.BeginScrollView(_audioCodecPos, GUILayout.Height(100));
				int newCodecIndex = GUILayout.SelectionGrid(-1, _audioCodecNames, 1) - 1;
				GUILayout.EndScrollView();
				
				if (newCodecIndex >= -1)
				{
					_movieCapture._audioCodecIndex = newCodecIndex;
					if (_movieCapture._audioCodecIndex >= 0)
						_movieCapture._audioCodecName = _audioCodecNames[_movieCapture._audioCodecIndex + 1];
					else
						_movieCapture._audioCodecName = "Uncompressed";
					
					_shownSection = -1;
				}
					
				GUILayout.Space(16f);
			}
			
			GUI.enabled = true;
			
			GUILayout.Space(16f);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Filename Prefix & Ext: ");
			_movieCapture._autoFilenamePrefix = GUILayout.TextField(_movieCapture._autoFilenamePrefix, 64);
			_movieCapture._autoFilenameExtension = GUILayout.TextField(_movieCapture._autoFilenameExtension, 8);
			GUILayout.EndHorizontal();
			GUILayout.Space(16f);
			GUILayout.Space(16f);
			
			GUILayout.Label("(Press Esc or CTRL-F5 to stop capture)");
			
			GUILayout.BeginHorizontal();
			if (!_movieCapture.IsCapturing())
			{
				if (GUILayout.Button("Start Capture"))
				{
					StartCapture();
				}
			}
			else
			{
				if (!_movieCapture.IsPaused())
				{
					if (GUILayout.Button("Pause Capture"))
					{
						PauseCapture();
					}
				}
				else
				{
					if (GUILayout.Button("Resume Capture"))
					{
						ResumeCapture();
					}					
				}
				
				if (GUILayout.Button("Stop Capture"))
				{
					StopCapture();
				}
			}
			GUILayout.EndHorizontal();
			
			if (_movieCapture.IsCapturing())
			{
				if (!string.IsNullOrEmpty(_movieCapture.LastFilePath))
				{
					GUILayout.Label("Writing file: '" + System.IO.Path.GetFileName(_movieCapture.LastFilePath) + "'");
				}				
			}
			else
			{
				if (!string.IsNullOrEmpty(_movieCapture.LastFilePath))
				{
					GUILayout.Label("Last file written: '" + System.IO.Path.GetFileName(_movieCapture.LastFilePath) + "'");
				}				
			}
		}
		
		GUILayout.EndVertical();
	}

	private void GUI_RecordingStatus()
	{
		GUILayout.Space(8.0f);
		GUILayout.Label("Output", "box");
		GUILayout.BeginVertical("box");

        DrawGuiField("Recording to", System.IO.Path.GetFileName(_movieCapture.LastFilePath));
		GUILayout.Space(8.0f);
		
		GUILayout.Label("Video", "box");
        DrawGuiField("Dimensions", _movieCapture.GetRecordingWidth() + "x" + _movieCapture.GetRecordingHeight() + " @ " + ((int)_movieCapture._frameRate).ToString() + "hz");
        DrawGuiField("Codec", _movieCapture._codecName);
		
		if (!_movieCapture._noAudio && _movieCapture._isRealTime)
		{
			GUILayout.Label("Audio", "box");
            DrawGuiField("Source", _movieCapture._audioDeviceName);
            DrawGuiField("Codec", _movieCapture._audioCodecName);
			if (_movieCapture._audioDeviceName == "Unity")
			{
                DrawGuiField("Sample Rate", _movieCapture._unityAudioSampleRate.ToString() + "hz");
                DrawGuiField("Channels", _movieCapture._unityAudioChannelCount.ToString());
			}
		}
		
		GUILayout.EndVertical();
		
		GUILayout.Space(8.0f);
		
		GUILayout.Label("Stats", "box");
		GUILayout.BeginVertical("box");

		if (_movieCapture._frameTotal >= (int)_movieCapture._frameRate * 2)
		{
			Color originalColor = GUI.color;
			float fpsDelta = (_movieCapture._fps - (int)_movieCapture._frameRate);
			GUI.color = Color.red;
			if (fpsDelta > -10)
				GUI.color = Color.yellow;
			if (fpsDelta > -2)
				GUI.color = Color.green;

            DrawGuiField("Capture Rate", _movieCapture._fps.ToString("F1") + " FPS");
			
			GUI.color = originalColor;
		}
		else
		{
            DrawGuiField("Capture Rate:", ".. FPS");
		}

        DrawGuiField("File Size", (int)(_lastFileSize / (1024 * 1024)) + "MB");
        DrawGuiField("Video Length", _lastEncodedMinutes + ":" + _lastEncodedSeconds + "s");
        DrawGuiField("Encoded Frames", _movieCapture.NumEncodedFrames.ToString());
		
		GUILayout.Label("Dropped Frames", "box");
        DrawGuiField("In Unity", _movieCapture.NumDroppedFrames.ToString());
        DrawGuiField("In Encoder ", _movieCapture.NumDroppedEncoderFrames.ToString());
		
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();

		if (!_movieCapture.IsPaused())
		{
			if (GUILayout.Button("Pause Capture"))
			{
				PauseCapture();
			}
		}
		else
		{
			if (GUILayout.Button("Resume Capture"))
			{
				ResumeCapture();
			}					
		}
		
		if (GUILayout.Button("Stop Capture"))
		{
			StopCapture();
		}

		GUILayout.EndHorizontal();
	}

    private void DrawGuiField(string a, string b)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(a);
        GUILayout.FlexibleSpace();
        GUILayout.Label(b);
        GUILayout.EndHorizontal();
    }

	private void StartCapture()
	{
		_lastFileSize = 0;
		_lastEncodedMinutes = _lastEncodedSeconds = 0;
		if (_whenRecordingAutoHideUI)
			_showUI = false;
		_movieCapture.StartCapture();
	}

	private void StopCapture()
	{
		_movieCapture.StopCapture();
	}

	private void ResumeCapture()
	{
		_movieCapture.ResumeCapture();
	}

	private void PauseCapture()
	{
		_movieCapture.PauseCapture();
	}

	void Update()
	{
		if (_whenRecordingAutoHideUI && !_showUI)
		{
			if (!_movieCapture.IsCapturing())
				_showUI = true;
		}
		
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F5))
		{
			if (_movieCapture.IsCapturing())
				_movieCapture.StopCapture();
		}

		if (_movieCapture.IsCapturing())
		{
			_lastFileSize = _movieCapture.GetCaptureFileSize();
			_lastEncodedSeconds = _movieCapture.TotalEncodedSeconds;
			_lastEncodedMinutes = _lastEncodedSeconds / 60;
			_lastEncodedSeconds = _lastEncodedSeconds % 60;
		}
	}
}