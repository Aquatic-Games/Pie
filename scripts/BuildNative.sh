#!/bin/bash

SPIRV_CROSS_NAME="SPIRV-Cross"
SPIRV_CROSS_LIB_NAME="libspirv-cross-c-shared.so"

SHADERC_NAME="shaderc"

if [ $# -ne 3 ]; then
  echo "Requires 3 parameters! <LIBS_LOCATION> <BUILD_LOCATION> <NATIVE_OUTPUT_LOCATION>"
  exit 1
fi

libs_location=$1
build_location=$2
native_output_location=$3

current_loc=$(pwd)

spirv_location="${current_loc}/${libs_location}/${SPIRV_CROSS_NAME}"
spirv_build_location="${current_loc}/${build_location}/${SPIRV_CROSS_NAME}"

full_output_path="${current_loc}/${native_output_location}"

echo "Creating ${SPIRV_CROSS_NAME} build directory."
mkdir -p "${spirv_build_location}" || exit 1

pushd "${spirv_build_location}" || exit 1

echo "Creating ${SPIRV_CROSS_NAME} build files."
cmake "${spirv_location}" -DCMAKE_BUILD_TYPE=Release -DSPIRV_CROSS_SHARED=ON -DSPIRV_CROSS_CLI=OFF || exit 1

echo "Building ${SPIRV_CROSS_NAME}."
cmake --build . --config Release || exit 1

popd || exit 1

echo "Creating native output location."
mkdir -p "${full_output_path}"

echo "Copying ${SPIRV_CROSS_LIB_NAME} to output location."
cp -L "${spirv_build_location}/${SPIRV_CROSS_LIB_NAME}" "${full_output_path}" || exit 1

echo "${SPIRV_CROSS_NAME} build successful!"