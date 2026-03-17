const API_BASE= 'https://localhost:7651/api';

async function apiFetch(url,options={}){
    const response=await fetch(url,{
        headers:{
            'Content-Type':'application/json'
            ...(options.headers || {})
        },
        credentials :'include'
        ...options
    });
    if(!response.ok){
        switch(response.status){
            case 401:
                throw new Error('Hibas felhasználónév vagy jelszó');
            case 403:
                throw new Error('Nincs jogosultságod');
            case 404:
                throw new Error('Nem található a felhasználó');
            default:
                throw new Error(`Hiba történt (${response.stasus}).`)
        }
    }
    const contentType = response.headers.get('Content-Type') || '';
    if(contentType.includes('application/json')){
        return await response.json();
    }
    return await response.text();
}
document.getElementById('login-form').addEventListener('submit',async e =>{
    e.preventDefault();
    const form=e.target;
    const data = Object.fromEntries(new FormData(form));
    const log=document.getElementById('login-log')
    log.textContent='';

    try{
       await apiFetch(
        `${API_BASE}/`
       )
    }
})
