CONSOLE_PATH='xut-console.exe'
IPA_PATH='XamarinFormsControlGalleryiOS-1.0.ipa'
APK_PATH='AndroidControlGallery.AndroidControlGallery-Signed.apk'
TEST_DIR_IOS='iOSUITestsAssemblyDir/'
TEST_DIR_ANDROID='AndroidUITestsAssemblyDir/'
ACCOUNT=''

if (( $# != 4 )); then
	echo "Usage sh testcloud-upload <PLATFORM> <DEVICES> <CATEGORY> <ASYNC>"
	echo "PLATFORM options: iOS, Android"
	echo "iOS DEVICES: 9b70f1c3 (one), 5d64e3b2 (min API subset), 9795e9c4 (larger subset), 7445fdcc (all devices)"
	echo "Android DEVICES: c070a268 (one), c0d2ef95 (min API subset), c87881ac (larger subset), 91c3565f (all devices)"
	echo "CATEGORY options: Layout, Control, View, Page, CellsListTable, Visual, Keyboard, IssueNamed, Issue1xxToIssue4xx, Issue5xxToIssue9xx"
	echo "ASYNC options: Sync (needed for test reporting in CI), Async (batch runs)"
	exit 1
fi

if [ "$1" != "iOS" ] && [ "$1" != "Android" ]; then
	echo "Unsupported PLATFORM: $1" 
	exit 1
fi

# Allowed CATEGORIES
declare -a categories=(
"Smoke"
"Layout"
"Control"
"View"
"Page"
"CellsListTable"
"Visual"
"Keyboard"
"ManualReview"
"IssueNamed"
"Issue1xxToIssue4xx"
"Issue5xxToIssue9xx"
)

validCategory=false
for category in "${categories[@]}"
do
	if [ $3 == "$category" ]; then
		validCategory=true
	fi
done

if [ "$validCategory" == false ]; then
	echo "Unsupported CATEGORY"
	exit 1
fi

if [ "$1" == "iOS" ]; then
	if [ "$4" == "Async" ]; then
		echo "Uploading Async: $1 $2 $3"
		mono $CONSOLE_PATH submit $IPA_PATH $ACCOUNT $2 --assembly-dir $TEST_DIR_IOS --series $3 --locale "en_US" --category $3 --async;
	else
		echo "Uploading Sync: $1 $2 $3"
		mono $CONSOLE_PATH submit $IPA_PATH $ACCOUNT $2 --assembly-dir $TEST_DIR_IOS --series $3 --locale "en_US" --category $3;
	fi
fi

if [ "$1" == "Android" ]; then
	if [ "$4" == "Async" ]; then
		echo "Uploading Async: $1 $2 $3"
		mono $CONSOLE_PATH submit $APK_PATH $ACCOUNT $2 --assembly-dir $TEST_DIR_ANDROID --series $3 --locale "en_US" --category $3 --async;
	else
		echo "Uploading Sync: $1 $2 $3"
		mono $CONSOLE_PATH submit $APK_PATH $ACCOUNT $2 --assembly-dir $TEST_DIR_ANDROID --series $3 --locale "en_US" --category $3;
	fi
fi

