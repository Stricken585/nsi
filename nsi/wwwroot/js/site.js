async function Import(dict) {
    setStatus('Импорт...', 'text-muted');
    const data = await fetchJson(`/Nsi/Import?dict=${dict}`);
    if (data) {
        setStatus(data.message, 'text-success');
        setTimeout(() => location.reload(), 1000);
    }
}

async function Clear(dict) {
    if (!confirm('Очистить таблицу?')) return;
    setStatus('Очиcтка...', 'text-muted');
    const data = await fetchJson(`/Nsi/Clear?dict=${dict}`);
    if (data) {
        setStatus(data.message, 'text-success');
        setTimeout(() => location.reload(), 1000);
    }
}

async function fetchJson(url) {
    try {
        const response = await fetch(url);
        return await response.json();
    } catch {
        setStatus('Ошибка запроса', 'text-danger');
        return null;
    }
}

function setStatus(text, cls) {
    const el = document.getElementById('status');
    el.textContent = text;
    el.className = `small ${cls}`;
}
