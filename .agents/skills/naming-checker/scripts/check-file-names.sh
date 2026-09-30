#!/usr/bin/env bash

set -euo pipefail

if (($# == 0)); then
  set -- .
fi

printf '| Severity | Category | Location | Current name | Rule | Suggested fix |\n'
printf '|---|---|---|---|---|---|\n'

finding_count=0

to_kebab_case() {
  local value=$1

  printf '%s' "$value" \
    | sed -E 's/([A-Z]+)([A-Z][a-z])/\1-\2/g; s/([a-z0-9])([A-Z])/\1-\2/g' \
    | tr '[:upper:]' '[:lower:]'
}

check_file() {
  local path=$1
  local filename stem suffix suggested escaped_path escaped_name escaped_suggestion

  filename=${path##*/}

  [[ $filename == *.agent.md ]] && return 0
  [[ $filename == *.test.* ]] && return 0

  stem=${filename%%.*}
  [[ $stem =~ ^[a-z][a-z0-9]*([A-Z][A-Za-z0-9]*)+$ ]] || return 0

  suffix=${filename#"$stem"}
  suggested="$(to_kebab_case "$stem")$suffix"
  escaped_path=${path//|/\\|}
  escaped_name=${filename//|/\\|}
  escaped_suggestion=${suggested//|/\\|}

  printf '| Warning | File | `%s:1` | `%s` | Lowercase-leading source files use kebab-case | Rename to `%s` and update references |\n' \
    "$escaped_path" "$escaped_name" "$escaped_suggestion"
  ((finding_count += 1))
}

for target in "$@"; do
  if [[ -f $target ]]; then
    check_file "$target"
    continue
  fi

  if [[ ! -d $target ]]; then
    printf 'naming-checker: path not found: %s\n' "$target" >&2
    exit 2
  fi

  while IFS= read -r -d '' path; do
    check_file "$path"
  done < <(
    find "$target" \
      \( -type d \( \
        -name .git -o \
        -name node_modules -o \
        -name bin -o \
        -name obj -o \
        -name dist -o \
        -name build -o \
        -name coverage \
      \) -prune \) -o \
      -type f -print0
  )
done

if ((finding_count == 0)); then
  printf '\nNo camelCase filenames requiring kebab-case were found.\n'
fi