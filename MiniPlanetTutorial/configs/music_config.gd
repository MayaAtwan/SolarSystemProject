class_name MusicConfig

enum Keys {
	Space
}

const FILE_PATHS := {
	Keys.Space : "res://audio/audioSpace.ogg"
}

static func get_audio_stream(key:Keys) -> AudioStream:
	return load(FILE_PATHS.get(key))
