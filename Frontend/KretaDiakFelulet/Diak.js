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

const diakOrarendAdat = {
    hetfo: {
        1: "Matematika",
        2: "Magyar",
        3: "Angol",
        4: "Történelem",
        5: "Informatika"
    },
    kedd: {
        1: "Fizika",
        2: "Matematika",
        3: "Biológia",
        4: "Angol"
    },
    szerda: {
        1: "Kémia",
        2: "Magyar",
        3: "Matematika",
        4: "Informatika",
        5: "Testnevelés"
    },
    csutortok: {
        1: "Történelem",
        2: "Fizika",
        3: "Matematika",
        4: "Földrajz"
    },
    pentek: {
        1: "Angol",
        2: "Magyar",
        3: "Osztályfőnöki",
        4: "Testnevelés"
    }
};

function diakOrarendMegjelenites(adat) {

    const container = document.getElementById("diakOrarendContainer");

    const napok = [
        { key: "hetfo", label: "Hétfő" },
        { key: "kedd", label: "Kedd" },
        { key: "szerda", label: "Szerda" },
        { key: "csutortok", label: "Csütörtök" },
        { key: "pentek", label: "Péntek" }
    ];

    const oraSzamok = [1,2,3,4,5,6,7,8];

    let html = `
        <div class="table-responsive">
        <table class="table table-bordered table-striped text-center align-middle">

        <thead class="table-dark">
        <tr>
        <th>Óra</th>
    `;

    napok.forEach(nap=>{
        html += `<th>${nap.label}</th>`;
    });

    html += `
        </tr>
        </thead>
        <tbody>
    `;

    oraSzamok.forEach(ora=>{
        html += `<tr><td><b>${ora}.</b></td>`;

        napok.forEach(nap=>{
            const tantargy = adat?.[nap.key]?.[ora] || "-";
            html += `<td>${tantargy}</td>`;
        });

        html += `</tr>`;
    });

    html += `
        </tbody>
        </table>
        </div>
    `;

    container.innerHTML = html;
}
document.addEventListener("DOMContentLoaded", function () {
    diakOrarendMegjelenites(diakOrarendAdat);
});

const jegyek = [

{
tantargy:"Matematika",
tipus:"Dolgozat",
jegy:5,
datum:"2026.03.01"
},

{
tantargy:"Matematika",
tipus:"Felelet",
jegy:4,
datum:"2026.03.05"
},

{
tantargy:"Magyar",
tipus:"Felelet",
jegy:4,
datum:"2026.03.02"
},

{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},

{
tantargy:"Történelem",
tipus:"Dolgozat",
jegy:5,
datum:"2026.03.06"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},
{
tantargy:"Magyar",
tipus:"Témazáró",
jegy:3,
datum:"2026.03.04"
},



];


function jegyekBetoltese(lista){

let tabla = document.getElementById("jegyekTabla");
tabla.innerHTML = "";

lista.forEach(jegy => {

tabla.innerHTML += `
<tr>
<td>${jegy.tantargy}</td>
<td>${jegy.tipus}</td>
<td><b>${jegy.jegy}</b></td>
<td>${jegy.datum}</td>
</tr>
`;

});

}


function tantargyAtlag(lista){

let targyak = {};

lista.forEach(j => {

if(!targyak[j.tantargy]){
targyak[j.tantargy] = [];
}

targyak[j.tantargy].push(j.jegy);

});


let listaElem = document.getElementById("atlagLista");
listaElem.innerHTML = "";


for(let targy in targyak){

let jegyek = targyak[targy];

let osszeg = 0;

jegyek.forEach(j => {
osszeg += j;
});

let atlag = osszeg / jegyek.length;

listaElem.innerHTML += `<b>${targy}</b>: ${atlag.toFixed(2)}`;

}

}


jegyekBetoltese(jegyek);
tantargyAtlag(jegyek);