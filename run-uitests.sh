sh ./run-ci-uitests-android.sh &
pid=$!
sh ./run-ci-uitests-ios.sh

# wait for background process if longer than foreground
wait $!
