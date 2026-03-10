function toggleSidebar(){

	const sidebar = document.getElementById("sidebar");
	const content = document.querySelector(".content");

	sidebar.classList.toggle("closed");
	content.classList.toggle("full");

}

const uzenetek = [
    {
    felado: "Osztályfőnök",
    tema: "Szülői értekezlet",
    datum: "2026.03.05",
    szoveg: "Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz."
    },
    
    {
    felado: "Igazgatóság",
    tema: "Tanév rendje",
    datum: "2026.03.04",
    szoveg: "A tavaszi szünet április 2-től kezdődik."
    },
    
    {
    felado: "Matektanár",
    tema: "Dolgozat",
    datum: "2026.03.03",
    szoveg: "A dolgozat jegyei felkerültek a rendszerbe."
    }
    ];
    
    function mutat(i){
    
    let u = uzenetek[i];
    
    document.getElementById("uzenet").innerHTML =
    "<h3>"+u.tema+"</h3>" +
    "<b>"+u.felado+"</b> - "+u.datum+
    "<p>"+u.szoveg+"</p>";
    
    }

    document.querySelectorAll("[data-view]").forEach(gomb => {
        gomb.addEventListener("click", function () {
            let id = this.getAttribute("data-view");
    
            document.querySelectorAll(".panel").forEach(panel => {
                panel.classList.remove("active");
            });
    
            document.getElementById(id).classList.add("active");
        });
    });




function kuldes(){

let cimzett = document.getElementById("cimzett").value;
let tema = document.getElementById("tema").value;
let szoveg = document.getElementById("szoveg").value;

if(cimzett === "" || tema === "" || szoveg === ""){

document.getElementById("uzenetStatus").innerHTML =
"<span style='color:red'>Minden mezőt ki kell tölteni!</span>";

return;

}

document.getElementById("uzenetStatus").innerHTML =
"<span style='color:green'>Üzenet elküldve!</span>";

document.getElementById("cimzett").value = "";
document.getElementById("tema").value = "";
document.getElementById("szoveg").value = "";

}


function modositas() {
    document.querySelectorAll("#lakcim, #email")
        .forEach(input => input.disabled = false);

    document.getElementById("mentesGomb").style.display = "inline-block";
}

function mentes() {
    document.querySelectorAll("#lakcim, #email")
        .forEach(input => input.disabled = true);

    document.getElementById("mentesGomb").style.display = "none";
}
