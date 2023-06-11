#!/bin/bash

for vert in *.vert; do
  file="${vert%.*}"
  glslc -o "${file}_vert.spv" "$vert"
done

for frag in *.frag; do
  file="${frag%.*}"
  glslc -o "${file}_frag.spv" "$frag"
done