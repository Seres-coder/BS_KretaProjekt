function toggleSidebar() {
    document.getElementById("sidebar").classList.toggle("closed");
}

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