CUR_DIR=$(dirname -- $0)
msbuild /nologo /v:minimal $CUR_DIR/UIViewLib.sln
mono --debug $CUR_DIR/../../build/lib/Debug/MonoEmbeddinator4000.exe --gen=objc -compile -p=macos -target=shared --out=uiviewlib_mac $CUR_DIR/bin/Debug/UIViewLib.dll
