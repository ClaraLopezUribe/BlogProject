let index = 0;

function addTag() {

    // Get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry");


    // Use search function to detect error state
    let searchResult = search(tagEntry.value);
    if (searchResult != null) {
        // Trigger sweet alert for error state
        swalWithDarkButton.fire({
            html: `<span class='font-weight-bolder'>${searchResult.toUpperCase()}</span>`
        });
    }
    else {

        // Create a new Select Option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;
    }

    // Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

//// CF solution:  
////// This implementation caused some unexpected behaviors (leaving blank spaces after deleting a tag on subsequent tag additions)
//function deleteTag() {

//    let tagCount = 1;
//    let tagList = document.getElementById("TagList");
//    if (!tagList) return false;

//    if (tagList.selectedIndex == -1) {
//        swalWithDarkButton.fire({
//            html: '<span class="font-weight-bolder">YOU MUST SELECT A TAG BEFORE DELETING</span>',
//        });
//        return true;
//    }

//    while (tagCount > 0) {
//        if (tagList.selectedIndex >= 0) {
//            tagList.options[tagList.selectedIndex] = null;
//            --tagCount;
//        }
//        else {
//            tagCount = 0;
//            index--;
//        }
//    }
//}


// Refactored deleteTag function:
function deleteTag() {

    let tagList = document.getElementById("TagList");
    let tagCount = tagList.options.length;
    let selectedIndex = tagList.selectedIndex;

    if (!tagList) return false;

    if (selectedIndex < 0) {

        swalWithDarkButton.fire({
            html: "<span class='font-weight-bolder'>PLEASE SELECT A TAG BEFORE DELETING</span>"
        });

        return true;
    }

    while (tagCount > 0) {

        tagList.removeChild(tagList.options[selectedIndex]);
        index--;   
    }
}


// On form submission, select all of the entries on select list - jquery

$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
})


// Look for the tagValues variable to see if it contains data
if (tagValues != '') {
    // If it does, split it into an array and add each value to the list
    let tagArray = tagValues.split(',');
    for (let loop = 0; loop < tagArray.length; loop++) {
        // Load up or Replace the options that we have
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}
function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newOption;
}


// The Search function will detect either an empty or duplicate Tag on this post and return an error string if an error is detected
function search(str) {
    if (str == "") {
        return "Empty tags are not permitted";
    }

    var tagsElement = document.getElementById("TagList");
    if (tagsElement) {
        let options = tagsElement.options;
        for (let index = 0; index < options.length; index++) {
            if (options[index].value == str) {

                return `#${str} already exists on this post. </br>Duplicates are not permitted`;

            }
        }
    }
}


const swalWithDarkButton = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-danger btn-outline-dark'
    },
    imageUrl: "/assets/img/noun-error-3194175.png",
    timer: 15000,
    buttonsStyling: false
});

















