document.addEventListener("DOMContentLoaded",
    function () {

        if (document.getElementById("submissionFilterButton") != null) {
            document.getElementById("submissionFilterButton").style.display = "none";
            document.getElementById("reportsFilterButton").style.display = "none";
            var elements =document.querySelectorAll("input[type='checkbox']");
            for (var i = 0; i < elements.length; i++) {
                elements[i].addEventListener("click",
                    function() {
                        if (this.name == "jobTypeFilter")
                            document.getElementById("submissionFilterButton").click();
                        else
                            document.getElementById("reportsFilterButton").click();
                    });
            }
        }
        // for feedback banner
        if (document.getElementById("noThanksLink") != null) {
            document.getElementById("noThanksLink").addEventListener("click",
                function() {
                    document.getElementById("feedbackBanner").style.display = "none";
                    event.preventDefault();
                    return false;
                });
        }
    }

);