document.addEventListener("DOMContentLoaded",
    function () {

        document.getElementById("submissionFilterButton").style.display = "none";
        document.getElementById("reportsFilterButton").style.display = "none";
        var elements = document.getElementsByTagName("input");
        for (var i = 0; i < elements.length; i++) {
            elements[i].addEventListener("click",
                function () {
                    if (this.name == "jobTypeFilter")
                        document.getElementById("submissionFilterButton").click();
                    else
                        document.getElementById("reportsFilterButton").click();
                });
        }
    }

);