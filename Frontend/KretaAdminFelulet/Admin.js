const API_BASE      = "https://localhost:7273/api/Data";
const TIMETABLE_API = "https://localhost:7273/api/TimeTable";
const MESSAGE_API   = "https://localhost:7273/api/Message";

/* Megkeres egy HTML elemet a CSS szelektor alapján.
   Ha az elem nem létezik a HTML-ben, hibát dob. */
function mustGet(selector) {
    const el = document.querySelector(selector);
    if (!el) throw new Error(`Hiányzó HTML elem: ${selector}`);
    return el;
}

/* DOM elemek */
const sidebar          = mustGet("#sidebar");
const sidebarToggle    = mustGet("#sidebar-toggle");
const panelGombok      = document.querySelectorAll("[data-panel-target]");
const cimzettSelect    = mustGet("#cimzett");
const temaInput        = mustGet("#tema");
const szovegTextarea   = mustGet("#szoveg");
const uzenetStatus     = mustGet("#uzenetStatus");
const diakValaszto     = mustGet("#diakValaszto");
const diakAdatokForm   = mustGet("#diakAdatokForm");
const diakNevInput     = mustGet("#diakNev");
const diakLakcimInput  = mustGet("#diakLakcim");
const diakEmailInput   = mustGet("#diakEmail");
const diakSzuloInput   = mustGet("#diakSzulo");
const diakMentesGomb   = mustGet("#diakMentesGomb");
const diakStatus       = mustGet("#diakStatus");
const osztalySelect    = mustGet("#osztalySelect");
const orarendContainer = mustGet("#orarendContainer");
const modOsztalySelect    = mustGet("#modOsztalySelect");
const modOrarendContainer = mustGet("#modOrarendContainer");
const modOrarendStatus    = mustGet("#modOrarendStatus");

/* Állapotkezelés */
let kivalasztottDiak = null;
let tanarokLista     = [];

/* DayOfWeek enum értékek — a backend számot vár, nem stringet */
const NAP_ENUM = {
    "Sunday": 0, "Monday": 1, "Tuesday": 2,
    "Wednesday": 3, "Thursday": 4, "Friday": 5, "Saturday": 6
};

/* Magyar napnevek megjelenítéshez */
const NAP_NEVEK = {
    "Monday":    "Hétfő",
    "Tuesday":   "Kedd",
    "Wednesday": "Szerda",
    "Thursday":  "Csütörtök",
    "Friday":    "Péntek",
    "Saturday":  "Szombat",
    "Sunday":    "Vasárnap"
};

/* Visszajelző függvények */
function showDiakError(msg)   { diakStatus.innerHTML       = `<span style="color:red">${msg}</span>`; }
function showDiakSuccess(msg) { diakStatus.innerHTML       = `<span style="color:green">${msg}</span>`; }
function showModError(msg)    { modOrarendStatus.innerHTML = `<span style="color:red">${msg}</span>`; }
function showModSuccess(msg)  { modOrarendStatus.innerHTML = `<span style="color:green">${msg}</span>`; }

/* Inicializálás */
document.addEventListener("DOMContentLoaded", async function () {
    sidebarGombInit();
    panelValtasInit();
    await Promise.all([
        cimzettekBetoltese(),
        diakokListaBetoltese(),
        tanarokBetoltese(),
        osztalyokBetoltese()
    ]);
});

/* Sidebar nyitó/záró gomb */
function sidebarGombInit() {
    sidebarToggle.addEventListener("click", function () {
        sidebar.classList.toggle("closed");
    });
}

/* Panel váltás — bal oldali nav gombok */
function panelValtasInit() {
    panelGombok.forEach(gomb => {
        gomb.addEventListener("click", function () {
            const panelId = this.getAttribute("data-panel-target");
            document.querySelectorAll(".panel").forEach(p => p.style.display = "none");
            const cel = document.getElementById(panelId);
            if (cel) cel.style.display = "block";
            panelGombok.forEach(x => x.classList.remove("active"));
            this.classList.add("active");
        });
    });
}

/* Cimzett select feltöltése tanárokkal és diákokkal */
async function cimzettekBetoltese() {
    cimzettSelect.innerHTML = '<option value="">-- Válassz cimzettet --</option>';
    try {
        const tanarRes = await fetch(`${API_BASE}/tanarlistazasa`, { credentials: "include" });
        if (tanarRes.ok) {
            const tanarok = await tanarRes.json();
            const csoport = document.createElement("optgroup");
            csoport.label = "── Tanárok ──";
            tanarok.forEach(t => {
                const opt = document.createElement("option");
                opt.value = t.user_id;
                opt.textContent = t.tanar_nev;
                csoport.appendChild(opt);
            });
            cimzettSelect.appendChild(csoport);
        }
        const diakRes = await fetch(`${API_BASE}/diaklistazasa`, { credentials: "include" });
        if (diakRes.ok) {
            const diakok = await diakRes.json();
            const csoport = document.createElement("optgroup");
            csoport.label = "── Diákok ──";
            diakok.forEach(d => {
                const opt = document.createElement("option");
                opt.value = d.user_id;
                opt.textContent = d.diak_nev;
                csoport.appendChild(opt);
            });
            cimzettSelect.appendChild(csoport);
        }
    } catch (e) { console.error("Cimzett betöltési hiba:", e); }
}

/* Üzenet elküldése POST kéréssel */
async function kuldes() {
    const tema     = temaInput.value.trim();
    const szoveg   = szovegTextarea.value.trim();
    const fogadoId = parseInt(cimzettSelect.value);

    if (!tema || !szoveg || !fogadoId || isNaN(fogadoId)) {
        uzenetStatus.innerHTML = "<span style='color:red'>Minden mezőt ki kell tölteni!</span>";
        return;
    }

    const mentettUser = localStorage.getItem("kretaUser");
    if (!mentettUser) { uzenetStatus.innerText = "Nincs bejelentkezve."; return; }
    const user   = JSON.parse(mentettUser);
    const userId = parseInt(user.id || user.Id);

    uzenetStatus.innerText = "Küldés...";
    try {
        const res = await fetch(`${MESSAGE_API}/messageadd`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ cim: tema, tartalom: szoveg, fogado_id: fogadoId, user_id: userId })
        });
        if (res.ok) {
            uzenetStatus.innerHTML = "<span style='color:green'>Üzenet elküldve!</span>";
            temaInput.value = "";
            szovegTextarea.value = "";
            cimzettSelect.value = "";
        } else {
            uzenetStatus.innerHTML = "<span style='color:red'>Hiba történt a küldés során.</span>";
        }
    } catch (e) {
        uzenetStatus.innerHTML = "<span style='color:red'>Nem sikerült kapcsolódni a szerverhez.</span>";
        console.error(e);
    }
}

/* Diák lista betöltése a select-be */
async function diakokListaBetoltese() {
    diakValaszto.innerHTML = '<option value="">-- Válassz diákot --</option>';
    try {
        const res = await fetch(`${API_BASE}/diaklistazasa`, { credentials: "include" });
        if (!res.ok) return;
        const diakok = await res.json();
        diakok.forEach(d => {
            const opt = document.createElement("option");
            opt.value = JSON.stringify(d);
            opt.textContent = d.diak_nev;
            diakValaszto.appendChild(opt);
        });
    } catch (e) { console.error("Diáklista hiba:", e); }
}

/* Diák kiválasztásakor feltölti az adatlapot az adataival */
function diakAdatokBetoltese() {
    if (!diakValaszto.value) { diakAdatokForm.style.display = "none"; return; }
    kivalasztottDiak = JSON.parse(diakValaszto.value);
    diakAdatokForm.style.display = "block";
    diakNevInput.value    = kivalasztottDiak.diak_nev  || "";
    diakLakcimInput.value = kivalasztottDiak.lakcim    || "";
    diakEmailInput.value  = kivalasztottDiak.emailcim  || "";
    diakSzuloInput.value  = kivalasztottDiak.szuloneve || "";
    document.querySelectorAll("#diakAdatokForm input").forEach(i => i.disabled = true);
    diakMentesGomb.style.display = "none";
    diakStatus.innerText = "";
}

/* Módosítás gomb — megnyitja a mezőket szerkesztésre */
function diakModositas() {
    document.querySelectorAll("#diakAdatokForm input:not(#diakOsztaly):not(#diakSzuletes)")
        .forEach(i => i.disabled = false);
    diakMentesGomb.style.display = "inline-block";
}

/* Mentés gomb — elküldi a módosított adatokat PUT kéréssel */
async function diakMentes() {
    if (!kivalasztottDiak) return;
    const dto = {
        diak_id:   kivalasztottDiak.diak_id,
        user_id:   kivalasztottDiak.user_id,
        diak_nev:  diakNevInput.value.trim(),
        lakcim:    diakLakcimInput.value.trim(),
        emailcim:  diakEmailInput.value.trim(),
        szuloneve: diakSzuloInput.value.trim()
    };
    diakStatus.innerText = "Mentés...";
    try {
        const res = await fetch(`${API_BASE}/modifystudentdata`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(dto)
        });
        if (res.ok) {
            showDiakSuccess("Sikeresen mentve!");
            document.querySelectorAll("#diakAdatokForm input").forEach(i => i.disabled = true);
            diakMentesGomb.style.display = "none";
            await diakokListaBetoltese();
        } else {
            showDiakError("Hiba történt a mentés során.");
        }
    } catch (e) { showDiakError("Nem sikerült kapcsolódni a szerverhez."); }
}

/* Törlés gomb — megerősítés után DELETE kéréssel törli a diákot */
async function diakTorles() {
    if (!kivalasztottDiak) return;
    if (!confirm(`Biztosan törlöd ${kivalasztottDiak.diak_nev} diákot?`)) return;
    try {
        const res = await fetch(`${API_BASE}/deletestudentdata?id=${kivalasztottDiak.diak_id}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (res.ok) {
            showDiakSuccess("Diák törölve!");
            diakAdatokForm.style.display = "none";
            await diakokListaBetoltese();
        } else {
            showDiakError("Hiba történt a törlés során.");
        }
    } catch (e) { showDiakError("Nem sikerült kapcsolódni a szerverhez."); }
}

/* Mindkét osztály select feltöltése (megtekintés + módosítás) */
async function osztalyokBetoltese() {
    try {
        const res = await fetch(`${API_BASE}/osztalylistazasa`, { credentials: "include" });
        if (!res.ok) return;
        const osztalyok = await res.json();
        [osztalySelect, modOsztalySelect].forEach(sel => {
            sel.innerHTML = '<option value="">-- Válassz osztályt --</option>';
            osztalyok.forEach(o => {
                const opt = document.createElement("option");
                opt.value = o.osztaly_id;
                opt.textContent = o.osztaly_nev;
                sel.appendChild(opt);
            });
        });
    } catch (e) { console.error("Osztálylista hiba:", e); }
}

/* Órarend megtekintése — csak olvasható tábla */
function orarendBetoltes() {
    const osztalyId = osztalySelect.value;
    if (!osztalyId) { orarendContainer.innerHTML = "<p>Kérlek válassz egy osztályt.</p>"; return; }

    fetch(`${TIMETABLE_API}/gettimetable?osztaly_id=${osztalyId}`, { credentials: "include" })
        .then(res => res.json())
        .then(adat => { orarendContainer.innerHTML = orarendTablaHTML(adat, false, ""); })
        .catch(() => { orarendContainer.innerHTML = "<p>Hiba történt.</p>"; });
}

/* Tanárok betöltése — az órarend módosításhoz kell (tantargy_nev mező is) */
async function tanarokBetoltese() {
    try {
        const res = await fetch(`${API_BASE}/tanarlistazasa`, { credentials: "include" });
        if (res.ok) tanarokLista = await res.json();
    } catch (e) { console.error("Tanárlista hiba:", e); }
}

/* Órarend módosítása — módosítás gombokkal ellátott tábla */
function modOrarendBetoltes() {
    const osztalyId  = modOsztalySelect.value;
    const osztalyNev = modOsztalySelect.options[modOsztalySelect.selectedIndex]?.text ?? "";
    if (!osztalyId) { modOrarendContainer.innerHTML = "<p>Kérlek válassz egy osztályt.</p>"; return; }

    fetch(`${TIMETABLE_API}/gettimetable?osztaly_id=${osztalyId}`, { credentials: "include" })
        .then(res => res.json())
        .then(adat => { modOrarendContainer.innerHTML = orarendTablaHTML(adat, true, osztalyNev); })
        .catch(() => { modOrarendContainer.innerHTML = "<p>Hiba történt.</p>"; });
}

/* Közös tábla generáló — modosGomb=true esetén minden sor kap egy Módosítás gombot */
function orarendTablaHTML(adat, modosGomb, osztalyNev) {
    let html = "";
    Object.entries(adat).forEach(([nap, orak]) => {
        html += `<table>
            <thead><tr>
                <th>Nap</th><th>Óra</th><th>Tantárgy</th><th>Tanár</th>
                ${modosGomb ? "<th></th>" : ""}
            </tr></thead><tbody>`;
        orak.forEach(o => {
            html += `<tr
                data-orarend-id="${o.orarend_id}"
                data-nap="${nap}"
                data-ora="${o.ora}"
                data-osztaly="${osztalyNev}">
                <td>${NAP_NEVEK[nap] ?? nap}</td>
                <td>${o.ora}</td>
                <td class="tantargy-cell">${o.tantargyNev}</td>
                <td class="tanar-cell">${o.tanarNev}</td>
                ${modosGomb
                    ? `<td><button
                            style="background:#f0a500;border:none;padding:4px 12px;border-radius:6px;cursor:pointer;font-weight:bold;"
                            onclick="orasorModositas(this)">Módosítás</button></td>`
                    : ""}
            </tr>`;
        });
        html += `</tbody></table>`;
    });
    return html;
}

/* Módosítás gombra a tanár cellát select-re cseréli.
   A tantárgy automatikusan frissül a kiválasztott tanár tantargy_nev-e alapján. */
function orasorModositas(btn) {
    const tr           = btn.closest("tr");
    const tanarCell    = tr.querySelector(".tanar-cell");
    const tantargyCell = tr.querySelector(".tantargy-cell");
    const jelenlegiTanar = tanarCell.textContent.trim();

    const opts = tanarokLista.map(t =>
        `<option value="${t.tanar_nev}" data-tantargy="${t.tantargy_nev ?? t.szak}"
            ${t.tanar_nev === jelenlegiTanar ? "selected" : ""}>${t.tanar_nev}</option>`
    ).join("");

    tanarCell.innerHTML = `<select style="width:100%;padding:3px;border-radius:5px;">${opts}</select>`;
    btn.textContent = "Mentés";
    btn.style.background = "#2e7d32";
    btn.style.color = "white";
    btn.setAttribute("onclick", "orasorMentes(this)");

    const selectEl = tanarCell.querySelector("select");
    tantargyCell.textContent = selectEl.options[selectEl.selectedIndex]?.dataset.tantargy ?? "";
    selectEl.addEventListener("change", function () {
        tantargyCell.textContent = this.options[this.selectedIndex]?.dataset.tantargy ?? "";
    });
}

/* Mentés gombra elküldi a módosított sort PUT kéréssel.
   A nap mezőt NAP_ENUM segítségével számmá alakítja,
   mert a C# DayOfWeek enum számot vár, nem stringet. */
async function orasorMentes(btn) {
    const tr           = btn.closest("tr");
    const tanarCell    = tr.querySelector(".tanar-cell");
    const tantargyCell = tr.querySelector(".tantargy-cell");

    const dto = {
        orarend_id:   parseInt(tr.dataset.orarendId),
        tanar_nev:    tanarCell.querySelector("select").value,
        tantargy_nev: tantargyCell.textContent.trim(),
        osztaly_nev:  tr.dataset.osztaly,
        nap:          NAP_ENUM[tr.dataset.nap],
        ora:          parseInt(tr.dataset.ora)
    };

    try {
        const res = await fetch(`${TIMETABLE_API}/modifytimetable`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(dto)
        });
        if (res.ok) {
            tanarCell.textContent = dto.tanar_nev;
            btn.textContent = "Módosítás";
            btn.style.background = "#f0a500";
            btn.style.color = "yellow";
            btn.setAttribute("onclick", "orasorModositas(this)");
            showModSuccess("Sikeresen mentve!");
        } else {
            showModError("Hiba történt a mentés során.");
        }
    } catch {
        showModError("Nem sikerült kapcsolódni a szerverhez.");
    }
}