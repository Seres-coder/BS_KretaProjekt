const API_BASE = "https://localhost:7273/api/Data";
const MESSAGE_API = "https://localhost:7273/api/Message";

document.addEventListener("DOMContentLoaded", async function () {
    sidebarGomb();
    panelValtas();
    cimzettekBetoltese();
    diakokListaBetoltese(); 
    fetch("https://localhost:7273/api/Data/tanarlistazasa", { credentials: "include" }).then(r => r.json()).then(data => { tanarokLista = data; }).catch(() => {});
    osztalyokBetoltese()
});

//#region  side bar mukodese
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

//#region uzenet irasa

async function cimzettekBetoltese() {
    const select = document.getElementById("cimzett");
    select.innerHTML = '<option value="">-- Válassz címzettet --</option>';

    try {
        // Tanárok betöltése
        const tanarRes = await fetch(`${API_BASE}/tanarlistazasa`, { credentials: "include" });
        if (tanarRes.ok) {
            const tanarok = await tanarRes.json();
            
            const tanarGroup = document.createElement("optgroup");
            tanarGroup.label = "── Tanárok ──";
            
            tanarok.forEach(t => {
                const option = document.createElement("option");
                option.value = t.user_id; 
                option.textContent = t.tanar_nev;
                tanarGroup.appendChild(option);
            });
            select.appendChild(tanarGroup);
        }

        // Diákok betöltése
        const diakRes = await fetch(`${API_BASE}/diaklistazasa`, { credentials: "include" });
        if (diakRes.ok) {
            const diakok = await diakRes.json();
            
            const diakGroup = document.createElement("optgroup");
            diakGroup.label = "── Diákok ──";
            
            diakok.forEach(d => {
                const option = document.createElement("option");
                option.value = d.user_id; 
                option.textContent = d.diak_nev;
                diakGroup.appendChild(option);
            });
            select.appendChild(diakGroup);
        }

    } catch (e) {
        console.error("Cimzett betöltési hiba:", e);
    }
}

async function kuldes() {
    const tema = document.getElementById("tema").value.trim();
    const szoveg = document.getElementById("szoveg").value.trim();
    const select = document.getElementById("cimzett");
    const fogadoId = parseInt(select.value);
    const status = document.getElementById("uzenetStatus");

    const mentettFelhasznalo = localStorage.getItem("kretaUser");
    if (!mentettFelhasznalo) { status.innerText = "Nincs bejelentkezve."; return; }

    const user = JSON.parse(mentettFelhasznalo);
    const userId = parseInt(user.id || user.Id);

    if (!tema || !szoveg || !fogadoId || isNaN(fogadoId)) {
        status.innerHTML = "<span style='color:red'>Minden mezőt ki kell tölteni!</span>";
        return;
    }

    const dto = {
        cim: tema,
        tartalom: szoveg,
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
            status.innerHTML = "<span style='color:green'>Üzenet elküldve!</span>";
            document.getElementById("tema").value = "";
            document.getElementById("szoveg").value = "";
            document.getElementById("cimzett").value = "";
        } else {
            status.innerHTML = "<span style='color:red'>Hiba történt a küldés során.</span>";
        }
    } catch (err) {
        status.innerHTML = "<span style='color:red'>Nem sikerült kapcsolódni a szerverhez.</span>";
        console.error(err);
    }
}

//#endregion



//#region  diak adat modositas
let kivalasztottDiak = null;

async function diakokListaBetoltese() {
    const select = document.getElementById("diakValaszto");
    select.innerHTML = '<option value="">-- Válassz diákot --</option>';

    try {
        const res = await fetch(`${API_BASE}/diaklistazasa`, { credentials: "include" });
        if (!res.ok) return;

        const diakok = await res.json();
        diakok.forEach(d => {
            const option = document.createElement("option");
            option.value = JSON.stringify(d); 
            option.textContent = d.diak_nev;
            select.appendChild(option);
        });
    } catch (e) {
        console.error("Diáklista hiba:", e);
    }
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

    // Mezők visszazárása ha másik diákot választ
    document.querySelectorAll("#diakAdatokForm input").forEach(i => i.disabled = true);
    document.getElementById("diakMentesGomb").style.display = "none";
    document.getElementById("diakStatus").innerText = "";
}

function diakModositas() {
    document.querySelectorAll("#diakAdatokForm input:not(#diakOsztaly):not(#diakSzuletes)").forEach(i => i.disabled = false);
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
    status.innerText = "Mentés...";

    try {
        const res = await fetch(`${API_BASE}/modifystudentdata`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(dto)
        });

        if (res.ok) {
            status.innerHTML = "<span style='color:green'>Sikeresen mentve!</span>";
            document.querySelectorAll("#diakAdatokForm input").forEach(i => i.disabled = true);
            document.getElementById("diakMentesGomb").style.display = "none";
            await diakokListaBetoltese(); // ✅ frissíti a listát
        } else {
            status.innerHTML = "<span style='color:red'>Hiba történt a mentés során.</span>";
        }
    } catch (e) {
        status.innerHTML = "<span style='color:red'>Nem sikerült kapcsolódni a szerverhez.</span>";
    }
}

async function diakTorles() {
    if (!kivalasztottDiak) return;
    if (!confirm(`Biztosan törlöd ${kivalasztottDiak.diak_nev} diákot?`)) return;

    const status = document.getElementById("diakStatus");

    try {
        const res = await fetch(`${API_BASE}/deletestudentdata?id=${kivalasztottDiak.diak_id}`, {
            method: "DELETE",
            credentials: "include"
        });

        if (res.ok) {
            status.innerHTML = "<span style='color:green'>Diák törölve!</span>";
            document.getElementById("diakAdatokForm").style.display = "none";
            await diakokListaBetoltese(); // ✅ frissíti a listát
        } else {
            status.innerHTML = "<span style='color:red'>Hiba történt a törlés során.</span>";
        }
    } catch (e) {
        status.innerHTML = "<span style='color:red'>Nem sikerült kapcsolódni a szerverhez.</span>";
    }
}

//#endregion 

//#region  orarend izek
function osztalyokBetoltese() {
    fetch("https://localhost:7273/api/Data/osztalylistazasa", { credentials: "include" })
        .then(res => res.json())
        .then(osztalyok => {
            let select = document.getElementById("osztalySelect");
            if (!select) return;
            select.innerHTML = '<option value="">-- Válassz osztályt --</option>';
            osztalyok.forEach(o => {
                let option = document.createElement("option");
                option.value = o.osztaly_id;
                option.textContent = o.osztaly_nev;
                select.appendChild(option);
            });
        });
}



function orarendBetoltes() {
    let osztalyId = document.getElementById("osztalySelect").value;
    let container = document.getElementById("orarendContainer");
    if (!osztalyId) { container.innerHTML = "<p>Kérlek válassz egy osztályt.</p>"; return; }

    fetch(`https://localhost:7273/api/TimeTable/gettimetable?osztaly_id=${osztalyId}`, { credentials: "include" })
        .then(res => res.json())
        .then(adat => {
            let html = "";
            Object.entries(adat).forEach(([nap, orak]) => {
                html += `<table>
                            <thead>
                                <tr>
                                    <th>Nap</th>
                                    <th>Óra</th>
                                    <th>Tantárgy</th>
                                    <th>Tanár</th>
                                </tr>
                            </thead>
                            <tbody>`;
                orak.forEach(o => {
                    html += `<tr>
                        <td>${NAP_NEVEK[nap] ?? nap}</td>
                        <td>${o.ora}</td>
                        <td>${o.tantargyNev}</td>
                        <td>${o.tanarNev}</td>
                    </tr>`;
                });
                html += `</tbody></table>`;
            });
            container.innerHTML = html;
        })
        .catch(() => { container.innerHTML = "<p>Hiba történt.</p>"; });
}

const NAP_NEVEK = {
    "Monday":    "Hétfő",
    "Tuesday":   "Kedd",
    "Wednesday": "Szerda",
    "Thursday":  "Csütörtök",
    "Friday":    "Péntek",
    "Saturday":  "Szombat",
    "Sunday":    "Vasárnap"
};
const NAP_ENUM  = { "Monday":1,"Tuesday":2,"Wednesday":3,"Thursday":4,"Friday":5 };



//#endregion