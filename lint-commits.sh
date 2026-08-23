#!/usr/bin/env bash

# Script para validar Conventional Commits usando pnpm dlx (o npx) sin dependencias locales.
# Uso:
#   ./lint-commits.sh                     -> Valida el último commit (HEAD~1..HEAD)
#   ./lint-commits.sh "feat: mi mensaje"  -> Valida el mensaje pasado por argumento
#   ./lint-commits.sh --branch            -> Valida todos los commits de la rama actual contra main
#   echo "mensaje" | ./lint-commits.sh    -> Valida desde stdin

set -e

if command -v pnpm >/dev/null 2>&1; then
  RUN_COMMITLINT="pnpm --package=@commitlint/cli --package=@commitlint/config-conventional dlx commitlint"
else
  RUN_COMMITLINT="npx --yes -p @commitlint/cli -p @commitlint/config-conventional commitlint"
fi

if [ -n "$1" ]; then
  if [ "$1" = "--branch" ]; then
    BASE_BRANCH="main"
    if ! git show-ref --verify --quiet refs/heads/$BASE_BRANCH; then
      BASE_BRANCH="master"
    fi
    echo "🔍 Validando commits de la rama contra '$BASE_BRANCH'..."
    $RUN_COMMITLINT --from="$BASE_BRANCH" --to=HEAD
  else
    echo "🔍 Validando mensaje: \"$1\"..."
    echo "$1" | $RUN_COMMITLINT
  fi
elif [ ! -t 0 ]; then
  # Entrada por pipe / stdin
  $RUN_COMMITLINT
else
  # Por defecto valida el último commit
  echo "🔍 Validando último commit local (HEAD)..."
  $RUN_COMMITLINT --from="HEAD~1"
fi

echo "✅ Commit(s) válido(s)."
