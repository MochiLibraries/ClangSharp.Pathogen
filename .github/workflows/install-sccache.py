#!/usr/bin/env python3
import hashlib
import io
import os
import stat
import sys
import tarfile
import urllib.request

import gha

# Configuration
url_base = "https://github.com/mozilla/sccache/releases/download/0.2.12/"

platform = sys.platform
if platform == 'win32':
    expected_hash = "fd05e91c59b9497d4ebae311b47a982f2a6eb942dca3c9c314cc1fb36f8bc64d"
    download_file_name = "sccache-0.2.12-x86_64-pc-windows-msvc"
    binary_name = "sccache.exe"
elif platform == 'linux':
    expected_hash = "26fd04c1273952cc2a0f359a71c8a1857137f0ee3634058b3f4a63b69fc8eb7f"
    download_file_name = "sccache-0.2.12-x86_64-unknown-linux-musl"
    binary_name = "sccache"
elif platform == 'darwin':
    expected_hash = "4945d295fa045dce9bf669d1313e4af4a504e74b08b61c1678902feb810c9ab7"
    download_file_name = "sccache-0.2.12-x86_64-apple-darwin"
    binary_name = "sccache"
else:
    assert(False), f"Unknown platform '{platform}'"

sccache_url = f"{url_base}{download_file_name}.tar.gz"
name_in_tar = f"{download_file_name}/{binary_name}"

# Figure out the sccache directory paths and create it if necessary
# We assume our working directory is the repo root.
# The binary is placed in a subdirectory of its download hash to ensure we don't re-use an old version.
sccache_root = os.path.join(os.getcwd(), "build-sccache")
sccache_cache_directory = os.path.join(sccache_root, "cache")

sccache_directory = os.path.join(sccache_root, expected_hash)
sccache_binary_location = os.path.join(sccache_directory, binary_name)
os.makedirs(sccache_directory, exist_ok=True)

# Add sccache to the workspace path and configure sccache's cache directory
gha.set_output('root-directory', sccache_root)
gha.set_environment_variable('SCCACHE_DIR', sccache_cache_directory)
gha.add_path(sccache_directory)

# If the output path already exists, no need to download
# (This will happen automagically because the binary is going to be cached along with the cache its self.)
if os.path.exists(sccache_binary_location):
    print("sccache already downloaded, won't download again.")
    sys.exit()

# Download the sccache archive
print(f"Download sccache from '{sccache_url}'...")
response = urllib.request.urlopen(sccache_url)
file_data = response.read()

# Validate the file hash
file_hash = hashlib.sha256(file_data).hexdigest()
assert(file_hash == expected_hash), f"Failed to validate '{sccache_url}', expected SHA256 hash {expected_hash} (got {file_hash})"

# Extract the sccache binary
tar_stream = io.BytesIO(file_data)
with tarfile.open(fileobj=tar_stream) as tar_file:
    with tar_file.extractfile(name_in_tar) as file:
        with io.open(sccache_binary_location, mode='wb') as output_file:
            output_file.write(file.read())

# Mark the binary as executable
if sys.platform != 'win32':
    current_mode = os.stat(sccache_binary_location).st_mode
    os.chmod(sccache_binary_location, current_mode | stat.S_IXUSR | stat.S_IXGRP | stat.S_IXOTH)

# Done
print("sccache downloaded.")
