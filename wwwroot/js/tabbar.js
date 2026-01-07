(function(){
  const items=[...document.querySelectorAll('.tabbar-item')];
  if(items.length===0) return;

  // apply min touch target
  items.forEach(i=>{ i.style.minWidth='64px'; i.style.minHeight='44px'; i.setAttribute('role','link'); });

  const setActive=(el)=>{
    items.forEach(i=> i.removeAttribute('aria-current'));
    el.setAttribute('aria-current','page');
    localStorage.setItem('tabbar:last', el.textContent.trim());
  };

  // set active based on URL or last stored
  const path = location.pathname.toLowerCase();
  const map=[
    {match: p=> p==='/' || p.includes('/home'), sel: ".tabbar a[asp-controller='Home']"},
    {match: p=> p.includes('/test'), sel: ".tabbar a[asp-controller='Test']"},
    {match: p=> p.includes('/students'), sel: ".tabbar a[asp-controller='Students']"},
    {match: p=> p.includes('/detail'), sel: ".tabbar a[asp-controller='Detail']"}
  ];
  let active = map.find(m=> m.match(path));
  let el = active ? document.querySelector(active.sel) : null;
  if(!el){
    const last = localStorage.getItem('tabbar:last');
    el = items.find(i=> i.textContent.trim()===last) || items[0];
  }
  if(el) setActive(el);

  // event handlers
  items.forEach(i=>{
    i.addEventListener('click',()=> setActive(i));
    i.addEventListener('pointerdown',(e)=>{
      const rect = i.getBoundingClientRect();
      i.style.setProperty('--x', `${e.clientX-rect.left}px`);
      i.style.setProperty('--y', `${e.clientY-rect.top}px`);
    }, {passive:true});
  });
})();