const API_BASE = "https://localhost:7273/api/Data";
const TIMETABLE_API = "https://localhost:7273/api/TimeTable";
const MESSAGE_API = "https://localhost:7273/api/Message";

document.addEventListener("DOMContentLoaded", async function () {
    sidebarGomb();
    panelValtas();
    getTeacherMessages();
    await tanarAdatokBetoltese();
});

//#region  uzenetek lekérése tanárnak
const listaUzenetek = document.querySelector("#lista");
const kuldoElem = document.querySelector("#uzenet-kuldo");
const targyElem = document.querySelector("#uzenet-targy");
const datumElem = document.querySelector("#uzenet-datum");
const tartalomElem = document.querySelector("#uzenet-tartalom");

async function getTeacherMessages() {
    const mentettFelhasznalo = localStorage.getItem("kretaUser");
    if (!mentettFelhasznalo) return;

    const user = JSON.parse(mentettFelhasznalo);
    const userId = user.id || user.Id;

    try {
       const res = await fetch(`${MESSAGE_API}/messageklistazasa?fogado_id=${userId}`, {
            method: "GET",
            credentials: "include"
        });


        if (!res.ok) {
            listaUzenetek.innerHTML = "<p>Nem sikerült lekérni az üzeneteket.</p>";
            return;
        }

        const uzenetek = await res.json();
        kiirUzenetek(uzenetek);
    } catch (error) {
        listaUzenetek.innerHTML = "<p>Hiba történt az üzenetek lekérése közben.</p>";
        console.error(error);
    }
}

function kiirUzenetek(uzenetek) {
    listaUzenetek.innerHTML = "";
    if (!uzenetek.length) {
        listaUzenetek.innerHTML = "<p>Nincsenek üzenetek.</p>";
        return;
    }

    uzenetek.forEach(u => {
        const gomb = document.createElement("button");
        gomb.className = "list-group-item list-group-item-action text-start";
        gomb.innerHTML = `
            <b>${u.kuldoname}</b><br>
            ${u.cim}<br>
            <small>${datum(u.kuldesidopontja)}</small>
        `;
        gomb.onclick = () => megjelenitUzenet(u);
        listaUzenetek.appendChild(gomb);
    });
}

function megjelenitUzenet(u) {
    kuldoElem.textContent = u.kuldoname;
    targyElem.textContent = u.cim;
    datumElem.textContent = datum(u.kuldesidopontja);
    tartalomElem.textContent = u.tartalom;
}

function datum(d) {
    return new Date(d).toLocaleDateString("hu-HU");
}

//#endregion


//#region sidebar működése és panel váltás

function sidebarGomb() {
    const sidebar = document.getElementById("sidebar");
    const gomb = document.getElementById("sidebar-toggle");

    if (sidebar && gomb) {
        gomb.addEventListener("click", function () {
            sidebar.classList.toggle("closed");
        });
    }
}

function panelValtas() {
    const gombok = document.querySelectorAll("[data-panel-target]");

    gombok.forEach(gomb => {
        gomb.addEventListener("click", function () {
            const panelId = this.getAttribute("data-panel-target");
            panelMutatas(panelId);

            gombok.forEach(x => x.classList.remove("active"));
            this.classList.add("active");
        });
    });
}

function panelMutatas(panelId) {
    const panelek = document.querySelectorAll(".panel");

    panelek.forEach(panel => {
        panel.style.display = "none";
    });

    const aktivPanel = document.getElementById(panelId);
    if (aktivPanel) {
        aktivPanel.style.display = "block";
    }
}

//#endregion

//#region tanár saját adatok betöltése

async function tanarAdatokBetoltese() {
    const mentettFelhasznalo = localStorage.getItem("kretaUser");

    if (!mentettFelhasznalo) {
        alert("Nincs bejelentkezve.");
        return;
    }

    const user = JSON.parse(mentettFelhasznalo);
    const userId = user.id || user.Id;

    try {
        const response = await fetch(`${API_BASE}/myteacherdata?userId=${userId}`, {
            method: "GET",
            credentials: "include"
        });

        if (!response.ok) {
            alert("Nem sikerült lekérni a tanár adatait.");
            return;
        }

        const teacherData = await response.json();

        tanarAdatokKiirasa(teacherData);

        if (teacherData.tanar_id) {
            await tanarOrarendBetoltese(teacherData.tanar_id);
        }

    } catch (error) {
        alert("Nem sikerült kapcsolódni a szerverhez.");
        console.error(error);
    }
}

function tanarAdatokKiirasa(teacherData) {
    document.getElementById("nev").value = teacherData.tanar_nev || "";
    document.getElementById("szak").value = teacherData.szak || "";
}

//#endregion

//#region tanár órarend betöltése

async function tanarOrarendBetoltese(tanarId) {
    try {
        const res = await fetch(`${TIMETABLE_API}/teachertimetabel?tanarId=${tanarId}`, {
            method: "GET",
            credentials: "include"
        });
        if (!res.ok) {
            document.getElementById("tanarOrarendContainer").innerHTML = "Nincs órarend.";
            return;
        }
        const adat = await res.json();
        console.log("Tanár órarend:", adat);
        tanarOrarendKiirasa(adat);
    } catch (error) {
        document.getElementById("tanarOrarendContainer").innerHTML = "Hiba történt.";
        console.error(error);
    }
}

function tanarOrarendKiirasa(orarend) {
    const container = document.getElementById("tanarOrarendContainer");
    const napok = Object.keys(orarend);

    if (!napok.length) {
        container.innerHTML = "Nincs órarend.";
        return;
    }

    let html = "";

    napok.forEach(nap => {
        html += `<h5 class="mt-3">${magyarNap(nap)}</h5>`;
        html += `
            <table class="table table-bordered table-striped mb-4">
                <thead>
                    <tr>
                        <th>Óra</th>
                        <th>Tantárgy</th>
                        <th>Osztály</th>
                    </tr>
                </thead>
                <tbody>
        `;

        orarend[nap].forEach(ora => {
            html += `
                <tr>
                    <td>${ora.ora}</td>
                    <td>${ora.tantargyNev}</td>
                    <td>${ora.osztalyNev}</td>
                </tr>
            `;
        });

        html += `
                </tbody>
            </table>
        `;
    });

    container.innerHTML = html;
}

function magyarNap(nap) {
    if (nap == "Monday" || nap == 1) return "Hétfő";
    if (nap == "Tuesday" || nap == 2) return "Kedd";
    if (nap == "Wednesday" || nap == 3) return "Szerda";
    if (nap == "Thursday" || nap == 4) return "Csütörtök";
    if (nap == "Friday" || nap == 5) return "Péntek";
    if (nap == "Saturday" || nap == 6) return "Szombat";
    if (nap == "Sunday" || nap == 0) return "Vasárnap";
    return nap;
}

//#endregion