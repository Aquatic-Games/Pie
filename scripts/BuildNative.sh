#!/bin/bash

SPIRV_CROSS_NAME="SPIRV-Cross"
SPIRV_CROSS_LIB_NAME="libspirv-cross-c-shared.so"

SHADERC_NAME="shaderc"
SHADERC_LIB_NAME="libshaderc_shared.so"

if [ $# -ne 3 ]; then
  echo "Requires 3 parameters! <LIBS_LOCATION> <BUILD_LOCATION> <NATIVE_OUTPUT_LOCATION>"
  exit 1
fi

libs_location=$1
build_location=$2
native_output_location=$3

current_loc=$(pwd)

shaderc_location="${current_loc}/${libs_location}/${SHADERC_NAME}"
shaderc_build_location="${current_loc}/${build_location}/${SHADERC_NAME}"

spirv_location="${current_loc}/${libs_location}/${SPIRV_CROSS_NAME}"
spirv_build_location="${current_loc}/${build_location}/${SPIRV_CROSS_NAME}"

full_output_path="${current_loc}/${native_output_location}"

echo "Creating ${SHADERC_NAME} build directory."
mkdir -p "${shaderc_build_location}" || exit 1

pushd "${shaderc_location}" || exit 1
./utils/git-sync-deps || exit 1
popd || exit 1

pushd "${shaderc_build_location}" || exit 1

echo "Creating ${SHADERC_NAME} build files."
# Ideally would append "|| exit 1" at the end, but this command encounters an error on windows which causes it to fail
cmake "${shaderc_location}" -DCMAKE_BUILD_TYPE=Release -DSHADERC_SKIP_INSTALL=1 -DSHADERC_SKIP_TESTS=1 -DSHADERC_SKIP_EXAMPLES=1 -DSHADERC_SKIP_COPYRIGHT_CHECK=1

echo "Building ${SHADERC_NAME}"
cmake --build . --config Release || exit 1

echo "${SHADERC_NAME} build successful!"

popd || exit 1

echo "Creating ${SPIRV_CROSS_NAME} build directory."
mkdir -p "${spirv_build_location}" || exit 1

pushd "${spirv_build_location}" || exit 1

echo "Creating ${SPIRV_CROSS_NAME} build files."
cmake "${spirv_location}" -DCMAKE_BUILD_TYPE=Release -DSPIRV_CROSS_SHARED=ON -DSPIRV_CROSS_CLI=OFF || exit 1

echo "Building ${SPIRV_CROSS_NAME}."
cmake --build . --config Release || exit 1

echo "${SPIRV_CROSS_NAME} build successful!"

popd || exit 1

echo "Creating native output location."
mkdir -p "${full_output_path}"

echo "Copying ${SHADERC_LIB_NAME} to output location."
find "${shaderc_build_location}" \( -iname "*.dll" -o -iname "*.so" \) -exec cp -L {} "${full_output_path}" \; || exit 1

echo "Copying ${SPIRV_CROSS_LIB_NAME} to output location."
find "${spirv_build_location}" \( -iname "*.dll" -o -iname "*.so" \) -exec cp -L {} "${full_output_path}" \; || exit 1

echo "Successful."