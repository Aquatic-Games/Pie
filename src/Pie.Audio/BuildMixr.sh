#!/bin/bash

echo "Building linux shared"
cargo build --manifest-path="../mixr/Cargo.toml" --lib --release
echo "Building windows dll"
cargo build --manifest-path="../mixr/Cargo.toml" --target x86_64-pc-windows-gnu --lib --release

cp ../mixr/target/release/libmixr.so ./libmixr.so
cp ../mixr/target/x86_64-pc-windows-gnu/release/mixr.dll ./mixr.dll