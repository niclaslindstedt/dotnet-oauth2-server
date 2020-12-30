#!/bin/sh

# This is a pre-commit hook that is meant to be installed in .git/hooks/

csharp_staged() {
  for f in `git diff --name-only --cached`; do
    if [[ ""$f"" =~ .cs$ ]]; then
      echo 1
      return
    fi
  done
  echo 0
}

if [[ $(csharp_staged) = 0 ]]; then
  echo ""Skipping linting -- no .cs files have been staged""
  exit 0
fi

dotnet regitlint \
    --solution-file=etimo-id.sln \
    --files-to-format=staged \
    --pattern=""**/*.cs"" \
    --fail-on-diff \
    --print-diff \
    --skip-tool-check \
    --jb --toolset=16.0 \
    --jb --verbosity=WARN \
    --jb --exclude=""**/Migrations/*"" \
  && exit 0