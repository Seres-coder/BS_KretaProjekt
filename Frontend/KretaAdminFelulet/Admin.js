const API_BASE = "https://localhost:7273/api/Data";
const USER_API = "https://localhost:7273/api/User";
const TIMETABLE_API = "https://localhost:7273/api/TimeTable";
const MESSAGE_API = "https://localhost:7273/api/Message";

let kivalasztottDiak = null;
let tanarokLista = [];

const NAP_ENUM = {
    "Sunday": 0, "Monday": 1, "Tuesday": 2,
    "Wednesday": 3, "Thursday": 4, "Friday": 5, "Saturday": 6
};

const NAP_NEVEK = {
    "Monday": "Hétfő", "Tuesday": "Kedd", "Wednesday": "Szerda",
    "Thursday": "Csütörtök", "Friday": "Péntek",
    "Saturday": "Szombat", "Sunday": "Vasárnap"
};

document.addEventListener("DOMContentLoaded", async function () {
    sidebarGombInit();
    panelValtasInit();

    await Promise.all([
        cimzettekBetoltese(),
        diakokListaBetoltese(),
        tanarokBetoltese(),
        osztalyokBetoltese(),
        regTantargyakBetoltese()
    ]);
});

//#region Sidebar és panel

function sidebarGombInit() {
    document.getElementById("sidebar-toggle").addEventListener("click", function () {
        document.getElementById("sidebar").classList.toggle("closed");
    });
}

function panelValtasInit() {
    const gombok = document.querySelectorAll("[data-panel-target]");

    gombok.forEach(gomb => {
        gomb.addEventListener("click", function () {
            const panelId = this.getAttribute("data-panel-target");

            document.querySelectorAll(".panel").forEach(p => {
                p.style.display = "none";
            });

            const cel = document.getElementById(panelId);
            if (cel) {
                cel.style.display = "block";
            }

            gombok.forEach(x => x.classList.remove("active"));
            this.classList.add("active");
        });
    });
}

//#endregion

//#region Üzenet írása

async function cimzettekBetoltese() {
    const select = document.getElementById("cimzett");
    if (!select) return;

    select.innerHTML = '<option value="">-- Válassz cimzettet --</option>';

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

        select.appendChild(csoport);
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

        select.appendChild(csoport);
    }
}

async function kuldes() {
    const tema = document.getElementById("tema").value.trim();
    const szoveg = document.getElementById("szoveg").value.trim();
    const select = document.getElementById("cimzett");
    const fogadoId = parseInt(select.value);
    const status = document.getElementById("uzenetStatus");

    const mentettUser = localStorage.getItem("kretaUser");
    if (!mentettUser) {
        status.innerText = "Nincs bejelentkezve.";
        return;
    }

    const user = JSON.parse(mentettUser);
    const userId = parseInt(user.id || user.Id);

    if (!tema || !szoveg || !fogadoId || isNaN(fogadoId)) {
        status.innerHTML = '<span class="text-danger">Minden mezőt ki kell tölteni!</span>';
        return;
    }

    const res = await fetch(`${MESSAGE_API}/messageadd`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
            cim: tema,
            tartalom: szoveg,
            fogado_id: fogadoId,
            user_id: userId
        })
    });

    if (res.ok) {
        status.innerHTML = '<span class="text-success">Üzenet elküldve!</span>';
        document.getElementById("tema").value = "";
        document.getElementById("szoveg").value = "";
        document.getElementById("cimzett").value = "";
    } else {
        status.innerHTML = '<span class="text-danger">Hiba történt a küldés során.</span>';
    }
}

//#endregion

//#region Diákok adatai

async function diakokListaBetoltese() {
    const select = document.getElementById("diakValaszto");
    if (!select) return;

    select.innerHTML = '<option value="">-- Válassz diákot --</option>';

    const res = await fetch(`${API_BASE}/diaklistazasa`, { credentials: "include" });
    if (!res.ok) return;

    const diakok = await res.json();

    diakok.forEach(d => {
        const opt = document.createElement("option");
        opt.value = JSON.stringify(d);
        opt.textContent = d.diak_nev;
        select.appendChild(opt);
    });
}

function diakAdatokBetoltese() {
    const select = document.getElementById("diakValaszto");
    const form = document.getElementById("diakAdatokForm");

    if (!select.value) {
        form.style.display = "none";
        return;
    }

    kivalasztottDiak = JSON.parse(select.value);
    form.style.display = "block";

    document.getElementById("diakNev").value = kivalasztottDiak.diak_nev || "";
    document.getElementById("diakLakcim").value = kivalasztottDiak.lakcim || "";
    document.getElementById("diakEmail").value = kivalasztottDiak.emailcim || "";
    document.getElementById("diakSzulo").value = kivalasztottDiak.szuloneve || "";

    document.querySelectorAll("#diakAdatokForm input").forEach(i => i.disabled = true);
    document.getElementById("diakMentesGomb").style.display = "none";
    document.getElementById("diakStatus").innerText = "";
}

function diakModositas() {
    document.querySelectorAll("#diakAdatokForm input").forEach(i => i.disabled = false);
    document.getElementById("diakMentesGomb").style.display = "inline-block";
}

async function diakMentes() {
    if (!kivalasztottDiak) return;

    const dto = {
        diak_id: kivalasztottDiak.diak_id,
        user_id: kivalasztottDiak.user_id,
        diak_nev: document.getElementById("diakNev").value.trim(),
        lakcim: document.getElementById("diakLakcim").value.trim(),
        emailcim: document.getElementById("diakEmail").value.trim(),
        szuloneve: document.getElementById("diakSzulo").value.trim()
    };

    const status = document.getElementById("diakStatus");

    const res = await fetch(`${API_BASE}/modifystudentdata`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(dto)
    });

    if (res.ok) {
        status.innerHTML = '<span class="text-success">Sikeresen mentve!</span>';
        document.querySelectorAll("#diakAdatokForm input").forEach(i => i.disabled = true);
        document.getElementById("diakMentesGomb").style.display = "none";
        await diakokListaBetoltese();
    } else {
        status.innerHTML = '<span class="text-danger">Hiba történt a mentés során.</span>';
    }
}

async function diakTorles() {
    if (!kivalasztottDiak) return;
    if (!confirm(`Biztosan törlöd ${kivalasztottDiak.diak_nev} diákot?`)) return;

    const status = document.getElementById("diakStatus");

    const res = await fetch(`${API_BASE}/deletestudentdata?id=${kivalasztottDiak.diak_id}`, {
        method: "DELETE",
        credentials: "include"
    });

    if (res.ok) {
        status.innerHTML = '<span class="text-success">Diák törölve!</span>';
        document.getElementById("diakAdatokForm").style.display = "none";
        await diakokListaBetoltese();
    } else {
        status.innerHTML = '<span class="text-danger">Hiba történt a törlés során.</span>';
    }
}

//#endregion

//#region Órarendek

async function osztalyokBetoltese() {
    const res = await fetch(`${API_BASE}/osztalylistazasa`, { credentials: "include" });
    if (!res.ok) return;

    const osztalyok = await res.json();

    ["osztalySelect", "modOsztalySelect", "regDiakOsztaly"].forEach(id => {
        const sel = document.getElementById(id);
        if (!sel) return;

        sel.innerHTML = '<option value="">-- Válassz osztályt --</option>';

        osztalyok.forEach(o => {
            const opt = document.createElement("option");
            opt.value = o.osztaly_id;
            opt.textContent = o.osztaly_nev;
            sel.appendChild(opt);
        });
    });
}

async function orarendBetoltes() {
    const osztalyId = document.getElementById("osztalySelect").value;
    const container = document.getElementById("orarendContainer");

    if (!osztalyId) {
        container.innerHTML = "<p>Kérlek válassz egy osztályt.</p>";
        return;
    }

    const res = await fetch(`${TIMETABLE_API}/gettimetable?osztaly_id=${osztalyId}`, {
        credentials: "include"
    });

    if (res.ok) {
        container.innerHTML = orarendTablaHTML(await res.json(), false, "");
    } else {
        container.innerHTML = "<p>Hiba történt.</p>";
    }
}

async function tanarokBetoltese() {
    const res = await fetch(`${API_BASE}/tanarlistazasa`, { credentials: "include" });
    if (res.ok) {
        tanarokLista = await res.json();
    }
}

async function modOrarendBetoltes() {
    const sel = document.getElementById("modOsztalySelect");
    const osztalyId = sel.value;
    const osztalyNev = sel.options[sel.selectedIndex]?.text ?? "";
    const container = document.getElementById("modOrarendContainer");

    if (!osztalyId) {
        container.innerHTML = "<p>Kérlek válassz egy osztályt.</p>";
        return;
    }

    const res = await fetch(`${TIMETABLE_API}/gettimetable?osztaly_id=${osztalyId}`, {
        credentials: "include"
    });

    if (res.ok) {
        container.innerHTML = orarendTablaHTML(await res.json(), true, osztalyNev);
    } else {
        container.innerHTML = "<p>Hiba történt.</p>";
    }
}

function orarendTablaHTML(adat, modosGomb, osztalyNev) {
    let html = "";

    Object.entries(adat).forEach(([nap, orak]) => {
        html += `<table class="table table-bordered table-striped text-center align-middle mb-2">
            <thead class="table-dark">
                <tr>
                    <th>Nap</th>
                    <th>Óra</th>
                    <th>Tantárgy</th>
                    <th>Tanár</th>
                    ${modosGomb ? "<th></th>" : ""}
                </tr>
            </thead>
            <tbody>`;

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
                ${modosGomb ? `<td><button class="btn btn-warning btn-sm fw-bold" onclick="orasorModositas(this)">Módosítás</button></td>` : ""}
            </tr>`;
        });

        html += `</tbody></table>`;
    });

    return html;
}

function orasorModositas(btn) {
    const tr = btn.closest("tr");
    const tanarCell = tr.querySelector(".tanar-cell");
    const tantargyCell = tr.querySelector(".tantargy-cell");
    const jelenlegiTanar = tanarCell.textContent.trim();

    const opts = tanarokLista.map(t =>
        `<option value="${t.tanar_nev}" data-tantargy="${t.tantargy_nev ?? t.szak}" ${t.tanar_nev === jelenlegiTanar ? "selected" : ""}>${t.tanar_nev}</option>`
    ).join("");

    tanarCell.innerHTML = `<select class="form-select form-select-sm">${opts}</select>`;

    btn.textContent = "Mentés";
    btn.className = "btn btn-success btn-sm fw-bold";
    btn.setAttribute("onclick", "orasorMentes(this)");

    const selectEl = tanarCell.querySelector("select");
    tantargyCell.textContent = selectEl.options[selectEl.selectedIndex]?.dataset.tantargy ?? "";

    selectEl.addEventListener("change", function () {
        tantargyCell.textContent = this.options[this.selectedIndex]?.dataset.tantargy ?? "";
    });
}

async function orasorMentes(btn) {
    const tr = btn.closest("tr");
    const tanarCell = tr.querySelector(".tanar-cell");
    const tantargyCell = tr.querySelector(".tantargy-cell");

    const dto = {
        orarend_id: parseInt(tr.dataset.orarendId),
        tanar_nev: tanarCell.querySelector("select").value,
        tantargy_nev: tantargyCell.textContent.trim(),
        osztaly_nev: tr.dataset.osztaly,
        nap: NAP_ENUM[tr.dataset.nap],
        ora: parseInt(tr.dataset.ora)
    };

    const res = await fetch(`${TIMETABLE_API}/modifytimetable`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(dto)
    });

    if (res.ok) {
        tanarCell.textContent = dto.tanar_nev;
        btn.textContent = "Módosítás";
        btn.className = "btn btn-warning btn-sm fw-bold";
        btn.setAttribute("onclick", "orasorModositas(this)");
        document.getElementById("modOrarendStatus").innerHTML = '<span class="text-success">Sikeresen mentve!</span>';
    } else {
        document.getElementById("modOrarendStatus").innerHTML = '<span class="text-danger">Hiba történt a mentés során.</span>';
    }
}

//#endregion

//#region Regisztráció

async function regTantargyakBetoltese() {
    const select = document.getElementById("regTanarTantargy");
    if (!select) return;

    const res = await fetch(`${API_BASE}/tantargylistazasa`, { credentials: "include" });
    if (!res.ok) return;

    const tantargyak = await res.json();

    select.innerHTML = '<option value="">-- Válassz tantárgyat --</option>';

    tantargyak.forEach(t => {
        const opt = document.createElement("option");
        opt.value = t.tantargy_nev;
        opt.textContent = t.tantargy_nev;
        select.appendChild(opt);
    });
}

async function diakRegisztracio() {
    const status = document.getElementById("regDiakStatus");

    const dto = {
        belepesnev: document.getElementById("regDiakBelepes").value.trim(),
        jelszo: document.getElementById("regDiakJelszo").value.trim(),
        diak_nev: document.getElementById("regDiakNev").value.trim(),
        emailcim: document.getElementById("regDiakEmail").value.trim(),
        lakcim: document.getElementById("regDiakLakcim").value.trim(),
        szuloneve: document.getElementById("regDiakSzulo").value.trim(),
        szuletesi_datum: document.getElementById("regDiakSzuletes").value,
        osztaly_id: parseInt(document.getElementById("regDiakOsztaly").value)
    };

    if (!dto.belepesnev || !dto.jelszo || !dto.diak_nev || !dto.emailcim ||
        !dto.lakcim || !dto.szuloneve || !dto.szuletesi_datum || !dto.osztaly_id) {
        status.innerHTML = '<span class="text-danger">Minden mezőt ki kell tölteni!</span>';
        return;
    }

    const res = await fetch(`${USER_API}/registerdiak`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(dto)
    });

    if (res.ok) {
        status.innerHTML = '<span class="text-success">Diák sikeresen regisztrálva!</span>';

        document.querySelectorAll("#diakRegPanel input").forEach(el => el.value = "");
        document.getElementById("regDiakOsztaly").value = "";
    } else {
        const hiba = await res.text();
        status.innerHTML = `<span class="text-danger">${hiba || "Hiba történt."}</span>`;
    }
}

async function tanarRegisztracio() {
    const status = document.getElementById("regTanarStatus");

    const dto = {
        belepesnev: document.getElementById("regTanarBelepes").value.trim(),
        jelszo: document.getElementById("regTanarJelszo").value.trim(),
        tanar_nev: document.getElementById("regTanarNev").value.trim(),
        szak: document.getElementById("regTanarSzak").value.trim(),
        tantargy_nev: document.getElementById("regTanarTantargy").value
    };

    if (!dto.belepesnev || !dto.jelszo || !dto.tanar_nev || !dto.szak || !dto.tantargy_nev) {
        status.innerHTML = '<span class="text-danger">Minden mezőt ki kell tölteni!</span>';
        return;
    }

    const res = await fetch(`${USER_API}/registertanar`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(dto)
    });

    if (res.ok) {
        status.innerHTML = '<span class="text-success">Tanár sikeresen regisztrálva!</span>';

        document.querySelectorAll("#tanarRegPanel input").forEach(el => el.value = "");
        document.getElementById("regTanarTantargy").value = "";
    } else {
        const hiba = await res.text();
        status.innerHTML = `<span class="text-danger">${hiba || "Hiba történt."}</span>`;
    }
}

//#endregion