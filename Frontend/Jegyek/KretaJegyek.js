function toggleSidebar(){
    document.getElementById("sidebar").classList.toggle("closed");
}


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

listaElem.innerHTML += `<li><b>${targy}</b>: ${atlag.toFixed(2)}</li>`;

}

}


jegyekBetoltese(jegyek);
tantargyAtlag(jegyek);