const API_BASE = "https://localhost:7273/api/Data";
const TIMETABLE_API = "https://localhost:7273/api/TimeTable";
const GRADE_API = "https://localhost:7273/api/Grade";
const MESSAGE_API="https://localhost:7273/api/Message";

document.addEventListener("DOMContentLoaded", async function () {
    sidebarGomb();
    panelValtas();
    await diakAdatokBetoltese();
    await tanarokBetoltese();
    
});
//#region  uzenetek lekérése
const lista = document.querySelector("#lista");
const kuldoElem = document.querySelector("#uzenet-kuldo");
const targyElem = document.querySelector("#uzenet-targy");
const datumElem = document.querySelector("#uzenet-datum");
const tartalomElem = document.querySelector("#uzenet-tartalom");

async function getMessages(diakId) {
    const res = await fetch(`${MESSAGE_API}/messageklistazasa?fogado_id=${diakId}`);
    const uzenetek = await res.json();
    kiir(uzenetek);
}

function kiir(uzenetek) {
    lista.innerHTML = "";
    uzenetek.forEach(u => {
        const gomb = document.createElement("button");
        gomb.className = "list-group-item list-group-item-action text-start";

        gomb.innerHTML = `
            <b>${u.kuldoname}</b><br>
            ${u.cim}<br>
            <small>${datum(u.kuldesidopontja)}</small>
        `;

        gomb.onclick = () => megjelenit(u);

        lista.appendChild(gomb);
    });
}

function megjelenit(u) {
    kuldoElem.textContent = u.kuldoname;
    targyElem.textContent = u.cim;
    datumElem.textContent = datum(u.kuldesidopontja);
    tartalomElem.textContent = u.tartalom;
}

function datum(d) {
    return new Date(d).toLocaleDateString("hu-HU");
}



//#endregion

//#region  uzenetek elkuldese

async function kuldes() {
    const tema = document.getElementById("tema").value.trim();
    const szoveg = document.getElementById("szoveg").value.trim();
    const select = document.getElementById("cimzett");
    const fogadoId = parseInt(select.value);
    const status = document.getElementById("uzenetStatus");

    const mentettFelhasznalo = localStorage.getItem("kretaUser");
    if (!mentettFelhasznalo) {
        status.innerText = "Nincs bejelentkezve.";
        return;
    }
    const user = JSON.parse(mentettFelhasznalo);
    const userId = parseInt(user.id || user.Id);

    if (!tema || !szoveg || !fogadoId || isNaN(fogadoId) || isNaN(userId)) {
        status.innerText = "Tölts ki minden mezőt, és válassz fogadót!";
        console.warn("Hiányzó vagy hibás mezők:", { tema, szoveg, fogadoId, userId });
        return;
    }

    const dto = {
        tartalom: szoveg,
        cim: tema,
        fogado_id: fogadoId,
        user_id: userId
    };
    status.innerText = "Küldés...";

    try {
        const res = await fetch(`${MESSAGE_API}/messageadd`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(dto)
        });

        if (res.ok) {
            status.innerText = "Üzenet elküldve!";
            document.getElementById("tema").value = "";
            document.getElementById("szoveg").value = "";

            
            await getMessages(userId);
        } else {
            const errorText = await res.text();
            console.error("Backend hiba:", errorText);
            status.innerText = "Hiba történt a küldés során.";
        }
    } catch (err) {
        console.error("Hálózati hiba:", err);
        status.innerText = "Nem sikerült kapcsolódni a szerverhez.";
    }
}

async function tanarokBetoltese() {
    const select = document.getElementById("cimzett");
    select.innerHTML = '<option value="">-- Válassz tanárt --</option>';

    const res = await fetch(`${API_BASE}/tanarlistazasa`, { credentials: "include" });
    if (!res.ok) return;

    const tanarok = await res.json();
    tanarok.forEach(tanar => {
        const option = document.createElement("option");
        option.value = tanar.user_id; 
        option.textContent = tanar.tanar_nev;
        select.appendChild(option);
    });
}
//#endregion 



//#region  sidebar mukodese es a panel valtas


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

//#region ezzel toltjuk be a sajat adatait egy diaknak 
async function diakAdatokBetoltese() {
    const mentettFelhasznalo = localStorage.getItem("kretaUser");
    if (!mentettFelhasznalo) {
        showMessage("adatokStatus", "Nincs bejelentkezve.");
        return;
    }
    const user = JSON.parse(mentettFelhasznalo);
    const userId = user.id || user.Id;
    try {
        const response = await fetch(`${API_BASE}/getmydata?user_id=${userId}`, {
            method: "GET",
            credentials: "include"
        });
        if (!response.ok) {
            showMessage("adatokStatus", "Nem sikerült lekérni az adatokat.");
            return;
        }
        const studentData = await response.json();
        if (studentData.osztaly_id) {
            await orarendBetoltese(studentData.osztaly_id);
        }
        adatokKiirasa(studentData);
        await jegyekBetoltese(studentData.diak_id);
        if (studentData.osztaly_id) {
            await orarendBetoltese(studentData.osztaly_id);
        }
        await getMessages(userId);
    } catch (error) {
        showMessage("adatokStatus", "Nem sikerült kapcsolódni a szerverhez.");
    }
}

function adatokKiirasa(studentData) {
    document.getElementById("nev").value = studentData.diak_nev || "";
    document.getElementById("osztaly").value = studentData.osztaly_id || "";
    document.getElementById("szuletes").value = datumFormazas(studentData.szuletesi_datum);
    document.getElementById("lakcim").value = studentData.lakcim || "";
    document.getElementById("email").value = studentData.emailcim || "";
    document.getElementById("szulo").value = studentData.szuloneve || "";
}

function datumFormazas(datum) {
    if (!datum) return "";
    const ujDatum = new Date(datum);
    const ev = ujDatum.getFullYear();
    const honap = String(ujDatum.getMonth() + 1).padStart(2, "0");
    const nap = String(ujDatum.getDate()).padStart(2, "0");
    return `${ev}-${honap}-${nap}`;
}

//#endregion

//#region  orarend betoltese a sajat diaknak
async function orarendBetoltese(osztalyId) {
    try {
        const res = await fetch(`${TIMETABLE_API}/gettimetable?osztaly_id=${osztalyId}`);
        if (!res.ok) {
            document.getElementById("diakOrarendContainer").innerHTML = "Nincs órarend.";
            return;
        }
        const adat = await res.json();
        orarendKiirasa(adat);
    } catch {
        document.getElementById("diakOrarendContainer").innerHTML = "Hiba történt.";
    }
}


function orarendKiirasa(orarend) {
    const container = document.getElementById("diakOrarendContainer");

    let html = "<table class='table table-bordered table-striped'>";

    const napok = Object.keys(orarend);

    napok.forEach((nap, index) => {
        if (index > 0) {
            html += `<tr><td colspan="4" style="height:8px;  border:none;"></td></tr>`;
        }

        orarend[nap].forEach(ora => {
            html += `
                <tr>
                    <td>${maggyara(nap)}</td>
                    <td><span class="ora-badge">${ora.ora}</td>
                    <td>${ora.tantargyNev}</td>
                    <td>${ora.tanarNev}</td>
                </tr>
            `;
        });
    });

    html += "</table>";
    container.innerHTML = html;
}
function maggyara(nap) {
    if (nap == "Monday") return "Hétfő";
    if (nap == "Tuesday") return "Kedd";
    if (nap == "Wednesday") return "Szerda";
    if (nap == "Thursday") return "Csütörtök";
    if (nap == "Friday") return "Péntek";
    return nap;
}
//#endregion

//#region  jegyek betoltese
async function jegyekBetoltese(diakId) {
    try {
        const res = await fetch(`${GRADE_API}/allgrade?id=${diakId}`, {
            method: "GET",
            credentials: "include" // <-- sends the auth cookie automatically
        });

        if (!res.ok) {
            document.getElementById("jegyekTabla").innerHTML = "<tr><td colspan='3'>Nincs jegy.</td></tr>";
            document.getElementById("atlagLista").innerHTML = "<li class='list-group-item'>Nincs adat</li>";
            return;
        }

        const jegyek = await res.json();
        jegyekKiirasa(jegyek);
        atlagokKiirasa(jegyek);
    } catch {
        document.getElementById("jegyekTabla").innerHTML = "<tr><td colspan='3'>Hiba történt.</td></tr>";
        document.getElementById("atlagLista").innerHTML = "<li class='list-group-item'>Hiba történt</li>";
    }
}
function jegyekKiirasa(jegyek) {
    const tabla = document.getElementById("jegyekTabla");
    let html = "";
    jegyek.forEach(jegy => {
        html += `
            <tr>
                <td>${jegy.tantargyNev}</td>
                <td>${jegy.ertek}</td>
                <td>${jegyDatum(jegy.datum)}</td>
            </tr>
        `;
    });
    tabla.innerHTML = html;
}

function atlagokKiirasa(jegyek) {
    const lista = document.getElementById("atlagLista");
    let html = "";
    let tantargyak = [];
    jegyek.forEach(jegy => {
        if (!tantargyak.includes(jegy.tantargyNev)) {
            tantargyak.push(jegy.tantargyNev);
        }
    });
    tantargyak.forEach(tantargy => {
        let osszeg = 0;
        let db = 0;
        jegyek.forEach(jegy => {
            if (jegy.tantargyNev === tantargy) {
                osszeg += jegy.ertek;
                db++;
            }
        });
        let atlag = (osszeg / db).toFixed(2);
        html += `<li class="list-group-item">${tantargy}: ${atlag}</li>`;
    });
    lista.innerHTML = html;
}
function jegyDatum(datum) {
    const ujDatum = new Date(datum);
    const ev = ujDatum.getFullYear();
    const honap = String(ujDatum.getMonth() + 1).padStart(2, "0");
    const nap = String(ujDatum.getDate()).padStart(2, "0");
    return `${ev}.${honap}.${nap}`;
}

//#endregion