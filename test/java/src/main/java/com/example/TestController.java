package com.example;

import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import io.jsonwebtoken.security.Keys;
import lombok.Data;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.security.Key;
import java.util.Date;
import java.util.Map;

@RestController
@RequestMapping("/api")
public class TestController {

    @GetMapping("/hello")
    public Map<String, String> hello() {
        return Map.of("message", "imports work");
    }

    @GetMapping("/token")
    public Map<String, String> token() {
        Key key = Keys.secretKeyFor(SignatureAlgorithm.HS256);
        String jwt = Jwts.builder()
                .setSubject("test")
                .setExpiration(new Date(System.currentTimeMillis() + 3600000))
                .signWith(key)
                .compact();
        return Map.of("token", jwt);
    }

    @GetMapping("/secure")
    @PreAuthorize("hasRole('USER')")
    public Map<String, String> secure() {
        return Map.of("status", "authorized");
    }
}

@Data
class TestDto {
    private String name;
    private int value;
}
