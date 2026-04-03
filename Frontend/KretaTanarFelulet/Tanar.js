const API_BASE = "https://localhost:7273/api/Data";
const TIMETABLE_API = "https://localhost:7273/api/TimeTable";
const MESSAGE_API = "https://localhost:7273/api/Message";
const GRADE_API = "https://localhost:7273/api/Grade";
let tanarAdat = null;

document.addEventListener("DOMContentLoaded", async function () {
    sidebarGomb();
    panelValtas();
    getTeacherMessages();
    await tanarAdatokBetoltese();
    await diakokBetolteseUzenethez();
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


//#region uzenet kuldese

async function diakokBetolteseUzenethez() {
    const select = document.getElementById("cimzett");
    select.innerHTML = '<option value="">-- Válassz diákot --</option>';

    const res = await fetch(`${API_BASE}/diaklistazasa`, { credentials: "include" });
    if (!res.ok) return;

    const diakok = await res.json();
    diakok.forEach(d => {
        const option = document.createElement("option");
        option.value = d.user_id; 
        option.textContent = d.diak_nev;
        select.appendChild(option);
    });
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
        status.innerText = "Tölts ki minden mezőt, és válassz diákot!";
        return;
    }

    const dto = { cim: tema, tartalom: szoveg, fogado_id: fogadoId, user_id: userId };
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
            await getTeacherMessages(); 
        } else {
            status.innerText = "Hiba történt a küldés során.";
        }
    } catch (err) {
        status.innerText = "Nem sikerült kapcsolódni a szerverhez.";
        console.error(err);
    }
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
        const response = await fetch(`${API_BASE}/getmyteacherdata?user_id=${userId}`, {
            method: "GET",
            credentials: "include"
        });

        if (!response.ok) {
            alert("Nem sikerült lekérni a tanár adatait.");
            return;
        }

        const teacherData = await response.json();
        tanarAdat = teacherData;        

        tanarAdatokKiirasa(teacherData);

        if (teacherData.tanar_id) {
            await tanarOrarendBetoltese(teacherData.tanar_id);
        }

        await diakokBetoltese();       

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
 
//#region jegyek kezelése

async function diakokBetoltese() {
	try {
		const res = await fetch(`${API_BASE}/diaklistazasa`, { credentials: "include" });
		if (!res.ok) return;
		const diakok = await res.json();
		const select = document.getElementById("diakValaszto");
		select.innerHTML = '<option value="">-- Válassz diákot --</option>';
		diakok.forEach(d => {
			const o = document.createElement("option");
			o.value = d.diak_id;
			o.textContent = d.diak_nev;
			select.appendChild(o);
		});
	} catch (e) {
		console.log(e);
	}
}

async function diakValasztva() {
	const select = document.getElementById("diakValaszto");
	const id = parseInt(select.value);
	const tabla = document.getElementById("jegyekTabla");
	tabla.innerHTML = "";
	if (!id) return;

	try {
		const res = await fetch(
			`${GRADE_API}/allgrade?tanar_id=${tanarAdat.tanar_id}`,
			{ credentials: "include" }
		);
		if (!res.ok) {
			tabla.innerHTML = "<tr><td colspan=4>Nincs jegy</td></tr>";
			return;
		}
		const jegyek = await res.json();
		const szurt = jegyek.filter(j => j.diak_id === id);
		if (!szurt.length) {
			tabla.innerHTML = "<tr><td colspan=4>Nincs jegy ehhez a diákhoz</td></tr>";
			return;
		}

		szurt.forEach(j => {
			tabla.innerHTML +=
				`<tr id="sor-${j.jegy_id}">\n` +
				`\t<td>${j.tantargyNev}</td>\n` +
				`\t<td>${j.ertek}</td>\n` +
				`\t<td>${jegyDatum(j.datum)}</td>\n` +
				`\t<td>\n` +
				`\t\t<button onclick="jegyModositas(${j.jegy_id}, ${j.ertek})">Módosít</button>\n` +
				`\t\t<button onclick="jegyTorles(${j.jegy_id})">Töröl</button>\n` +
				`\t</td>\n` +
				`</tr>\n`;
		});
	} catch (e) {
		console.log(e);
	}
}

async function jegyHozzaadas() {
	const select = document.getElementById("diakValaszto");
	const nev = select.options[select.selectedIndex]?.text;
	const ertek = parseInt(document.getElementById("ujJegy").value);
	const status = document.getElementById("jegyStatus");

	if (!nev || nev == "-- Válassz diákot --") {
		status.textContent = "Válassz diákot!";
		return;
	}
	if (!ertek || ertek < 1 || ertek > 5) {
		status.textContent = "Jegy 1-5";
		return;
	}

	const dto = {
		tanar_nev: tanarAdat.tanar_nev,
		tantargy_nev: "",
		diak_nev: nev,
		ertek: ertek
	};

	try {
		const res = await fetch(`${GRADE_API}/gradeadd`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			credentials: "include",
			body: JSON.stringify(dto)
		});

		if (res.ok) {
			status.textContent = "Felvéve!";
			document.getElementById("ujJegy").value = "";
			await diakValasztva();
		} else {
			const e = await res.text();
			status.textContent = "Hiba: " + e;
		}
	} catch {
		status.textContent = "Hiba a szerverrel";
	}
}

async function jegyModositas(id, regi) {
	const uj = parseInt(prompt(`Új jegy 1-5, most: ${regi}`));
	if (isNaN(uj) || uj < 1 || uj > 5) return;

	const dto = {
		jegy_id: id,
		ertek: uj,
		updatedatum: new Date().toISOString()
	};

	try {
		const res = await fetch(`${GRADE_API}/grademodify`, {
			method: "PUT",
			headers: { "Content-Type": "application/json" },
			credentials: "include",
			body: JSON.stringify(dto)
		});
		if (res.ok) await diakValasztva();
	} catch {}
}

async function jegyTorles(id) {
	if (!confirm("Biztos?")) return;
	try {
		const res = await fetch(`${GRADE_API}/gradedelete?id=${id}`, {
			method: "DELETE",
			credentials: "include"
		});
		if (res.ok) await diakValasztva();
	} catch {}
}

function jegyDatum(d) {
	const dd = new Date(d);
	return `${dd.getFullYear()}.${String(dd.getMonth() + 1).padStart(2, "0")}.${String(
		dd.getDate()
	).padStart(2, "0")}`;
}

//#endregion