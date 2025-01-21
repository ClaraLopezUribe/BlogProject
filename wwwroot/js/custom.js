let index = 0;

function addTag() {

    // Get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry");

    // Create a new Select Option
    let newOption = new Option(tagEntry.value, tagEntry.value);
    document.getElementById("TagList").options[index++] = newOption;

    // Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

// CF solution:
//function deleteTag() {

//    let tagCount = 1;

//    while (tagCount > 0) {
//        let tagList = document.getElementById("TagList");
//        let selectedIndex = tagList.selectedIndex;
//        if (selectedIndex >= 0) {
//            tagList.options[selectedIndex] = null;
//            --tagCount;
//        }
//        else {
//            tagCount = 0;
//            index--;
//        }
//    }
//}

// CF solution w/Tweeks
function deleteTag() {

    let tagList = document.getElementById("TagList");
    let tagCount = tagList.options.length;

    while (tagCount > 0) {
        
        let selectedIndex = tagList.selectedIndex;

        if (selectedIndex >= 0) {
            tagList.removeChild(tagList.options[selectedIndex]);
            index--;
        }
        else {
            tagCount = 0;
        }
    }
}



////My Solution:
//function deleteTag() {

//    let tagList = document.getElementById("TagList");

    
//    while (index >= 0) {

//        let selectedIndex = tagList.selectedIndex;
//        if (selectedIndex >= 0) {
//            tagList.removeChild(tagList.options[selectedIndex]);
//            index--;
//        }
//    }
//}