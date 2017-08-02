#!/bin/bash -e



function selfdir { (cd "$(dirname "$1")"; echo "$PWD"; ) }



selfdir=$(selfdir "$0")



base_url=https://bosstoragemirror.blob.core.windows.net/wrench/provisionator/19de0c2101229465240b09b8f720788ce8191937937500804cf4f14248f5b65e

latest_version_url="${base_url}/latest/version"



binary_name=provisionator

binary_path="${selfdir}/${binary_name}"

binary_url="${base_url}/latest/${binary_name}"



function update_in_place {

  curl -o "$binary_path" "$binary_url"

  chmod +x "$binary_path"

}



if [ -f "$binary_path" ]; then

  latest_version="$(curl -sL "${latest_version_url}")"

  chmod +x "$binary_path"

  current_version="$("$binary_path" -version 2>&1 || true)"

  if [ "$latest_version" != "$current_version" ]; then

    update_in_place

  fi

else

  update_in_place

fi



"$binary_path" "$@"