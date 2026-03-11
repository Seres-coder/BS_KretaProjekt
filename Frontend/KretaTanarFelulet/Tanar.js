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



const tanariOrarend = {
    tanar: {
        nev: "Seres Dániel",
        szakok: ["Matek", "Fizika"]
    },
    orak: {
        hetfo: {
            1: "Matek",
            3: "Fizika",
            5: "Matek"
        },
        kedd: {
            2: "Fizika",
            4: "Matek"
        },
        szerda: {
            1: "Matek"
        },
        csutortok: {
            3: "Fizika",
            6: "Matek"
        },
        pentek: {
            2: "Matek",
            4: "Fizika"
        }
    }
};

function tanariOrarendMegjelenites(adat) {
    const tanarInfo = document.getElementById("tanarInfo");
    const orarendContainer = document.getElementById("orarendContainer");

    if (!adat || !adat.tanar || !adat.orak) {
        tanarInfo.innerHTML = "";
        orarendContainer.innerHTML = '<p class="text-danger">Nincs betölthető órarend adat.</p>';
        return;
    }

    tanarInfo.innerHTML = `
        <h4 class="mb-2">${adat.tanar.nev}</h4>
        <p class="mb-0"><b>Szakok:</b> ${adat.tanar.szakok.join(", ")}</p>
    `;

    const napok = [
        { key: "hetfo", label: "Hétfő" },
        { key: "kedd", label: "Kedd" },
        { key: "szerda", label: "Szerda" },
        { key: "csutortok", label: "Csütörtök" },
        { key: "pentek", label: "Péntek" }
    ];

    const oraSzamok = [1, 2, 3, 4, 5, 6, 7, 8];

    let html = `
        <div class="table-responsive">
            <table class="table table-bordered table-striped align-middle">
                <thead class="table-dark">
                    <tr>
                        <th>Óra</th>
    `;

    napok.forEach(nap => {
        html += `<th>${nap.label}</th>`;
    });

    html += `
                    </tr>
                </thead>
                <tbody>
    `;

    oraSzamok.forEach(ora => {
        html += `<tr><td><b>${ora}.</b></td>`;

        napok.forEach(nap => {
            const tantargy = adat.orak?.[nap.key]?.[ora] || "-";
            html += `<td>${tantargy}</td>`;
        });

        html += `</tr>`;
    });

    html += `
                </tbody>
            </table>
        </div>
    `;

    orarendContainer.innerHTML = html;
}

document.addEventListener("DOMContentLoaded", function () {
    tanariOrarendMegjelenites(tanariOrarend);
});

const osztalyok = [
    ["Kiss Péter", "Nagy Anna", "Tóth Bence"],
    ["Szabó Lili", "Farkas Dávid", "Molnár Zsófi"],
    ["Varga Márk", "Papp Réka", "Balogh Noel"]
];

function osztalyBetolt(szam, nev) {

    document.getElementById("osztalyNev").innerText = nev;
    let tabla = document.getElementById("tabla");
    tabla.innerHTML = "";

    for (let diak of osztalyok[szam]) {

        tabla.innerHTML += `
        <tr>
            <td>${diak}</td>
            <td><select class="form-select"><option>Dolgozat</option><option>Felelet</option><option>Témazáró</option><option>Röpdolgozat</option><option>Házi feladat</option></select></td>
            <td><select class="form-select"><option>1</option><option>2</option><option>3</option><option>4</option><option>5</option></select></td>
            <td><input type="date" class="form-control"></td>
            <td><button class="btn btn-primary">Mentés</button></td>
        </tr>`;
    }
}
function mentes(i) {
    let nev = document.querySelectorAll("#tabla tr td:first-child")[i].innerText;
    let tantargy = document.getElementById("tantargy").value;
    let tipus = document.getElementById("tipus" + i).value;
    let jegy = document.getElementById("jegy" + i).value;
    let datum = document.getElementById("datum" + i).value;

    alert(
        "Elmentve\n\n" +
        "Név: " + nev + "\n" +
        "Tantárgy: " + tantargy + "\n" +
        "Típus: " + tipus + "\n" +
        "Jegy: " + jegy + "\n" +
        "Dátum: " + datum
    );
}

osztalyBetolt(0, "9.A");